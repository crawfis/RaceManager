using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using HardCard.Scoring;
using OhioState.Collections;

namespace RaceResults
{
    public partial class RaceResultsForm : Form
    {
        // The DataContext for the database
        private HardcardDataContext database = new HardcardDataContext();
        // NetworkListener takes the network packets and fires events.
        private NetworkListener listener;
        // ProcessBufferedReadings takes tag events and determines passing events.
        private ProcessBufferedReadings passDetector;
        // We will start the race (the networkListener) in a separate thread from
        //    the GUI thread.
        private Thread racingThread;
        private bool raceStarted = false;
        // TagSubscriber handles the middle-ware on connecting to the listener
        //   and calling the user-defined function to handle the event.
        private TagSubscriber readingsLogger;
        private TagSubscriber passingsLogger;
        private LoggerBinary binaryLogger;
        // Rather than marshal the reading or passing events to the GUI thread,
        //    we will buffer them and handle them on a timer tick.
        private IPriorityCollection<TagInfo> readingsQueue;
        private IPriorityCollection<TagInfo> passingsQueue;

        #region Start-Up
        /// <summary>
        /// Constructor.
        /// </summary>
        public RaceResultsForm()
        {
            InitializeComponent();
            // Wire-up the listeners, passdetector and loggers.
            SetupLogger();
            // Create the queues to handle thread-safe queuing.
            readingsQueue = new PriorityCollectionNonBlocking<TagInfo>("Queue", 1024);
            passingsQueue = new PriorityCollectionNonBlocking<TagInfo>("Queue", 1024);
            // Start the GUI update timer.
            viewUpdateTimer.Start();
        }

        private void SetupLogger()
        {
            readingsLogger = new TagSubscriber(LogReadings);
            passingsLogger = new TagSubscriber(LogPassings);
            binaryLogger = new LoggerBinary("out.bin");
        }

        private void StartRace()
        {
            // TODO: This should be put in a logger
            statusTextBox.AppendText(Environment.NewLine);
            statusTextBox.AppendText("Race Started at: ");
            statusTextBox.AppendText(DateTime.Now.ToLongTimeString());
            statusTextBox.AppendText(", ");
            statusTextBox.AppendText(DateTime.Now.ToLongDateString());
            statusTextBox.AppendText(Environment.NewLine);

            // Create a new thread to start handle the race logic.
            racingThread = new Thread(StartRaceThread);
            racingThread.Start();
        }

        private void StartRaceThread()
        {
            listener = new NetworkListener("Race Host");
            int initialiCapacity = 10000;
            PriorityCollectionBlocking<TagInfo> queue = new PriorityCollectionBlocking<TagInfo>("Queue", initialiCapacity);
            // The queue above is used to communicate between the buffer below and the passDetector.
            BufferReadings buffer = new BufferReadings(queue);
            passDetector = new ProcessBufferedReadings("Pass Detector", queue);
            buffer.AddPublisher(listener);
            // Subscribe the loggers to the publishers.
            readingsLogger.AddPublisher(listener);
            binaryLogger.AddPublisher(listener);
            passingsLogger.AddPublisher(passDetector);
            // Start the pass detection in its own thread.
            passDetector.Start();
            // Start the networkListener.
            listener.Start();
        }

        private readonly bool useExistingDatabase = true;
        private void RaceResultsForm_Load(object sender, EventArgs e)
        {
            if (useExistingDatabase)
            {
                // Display the current database records.
                tagReadingBindingSource.DataSource = from reading in database.TagReadings
                                                     orderby reading.ReadingNumber
                                                     select reading;
                passingBindingSource.DataSource = from passing in database.Passings
                                                  orderby passing.PassingNumber
                                                  select passing;
            }
            else
            {
                // Clear the database.
                var readings = from reading in database.TagReadings select reading;
                database.TagReadings.DeleteAllOnSubmit(readings);
                var passings = from passing in database.Passings select passing;
                database.Passings.DeleteAllOnSubmit(passings);
                database.SubmitChanges();
            }
        }

        // On first click, the start button will start the race.
        // It will then change the text to Stop Race. Clicking it
        // a second time will stop the race and disable the button.
        private void startButton_Click(object sender, EventArgs e)
        {
            if (!raceStarted)
            {
                StartRace();
                raceStarted = true;
                startButton.Text = "Stop Race";
            }
            else
            {
                EndRace();
                raceStarted = false;
                SubmitDBChanges();
                startButton.Text = "Race Ended";
                startButton.Enabled = false;
            }
        }
        #endregion Start-up

        #region Running the Race
        private void LogReadings(TagReadEventArgs e)
        {
            readingsQueue.Put(e.TagInfo);
        }

        private void LogPassings(TagReadEventArgs e)
        {
            passingsQueue.Put(e.TagInfo);
        }

        // This message should be called when a passing is saved.
        private void bindingNavigatorSaveItem1_Click(object sender, EventArgs e)
        {
            SubmitDBChanges();
        }

        private void SubmitDBChanges()
        {
            // In case the user was typing in a passing, we need to validate it.
            Validate();
            passingBindingSource.EndEdit();
            // Commits the changes to the database (rather than the BindingSource).
            database.SubmitChanges();
        }

        private void viewUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (readingsQueue.Count > 0 || passingsQueue.Count > 0)
            {
                while (readingsQueue.Count != 0)
                {
                    TagInfo tagInfo = readingsQueue.GetNext();
                    TagReading reading = new TagReading();
                    reading.Antenna = (short)tagInfo.Antenna;
                    reading.Frequency = tagInfo.Frequency;
                    reading.SignalStrength = tagInfo.SignalStrenth;
                    reading.Tag = tagInfo.ID.Value;
                    reading.Time = tagInfo.Time;
                    tagReadingBindingSource.Add(reading);
                }
                while (passingsQueue.Count != 0)
                {
                    TagInfo tagInfo = passingsQueue.GetNext();
                    Passing passing = new Passing();
                    passing.Tag = tagInfo.ID.Value;
                    passing.PassingTime = tagInfo.Time;
                    passing.ReadingNumber = -1;
                    passing.Lap = -1;
                    passingBindingSource.Add(passing);
                }
                database.SubmitChanges();

                passingDataGridView.Invalidate();
                tagReadingDataGridView.Invalidate();
            }
        }
        #endregion Running

        #region Ending
        private void RaceResultsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (raceStarted)
            {
                EndRace();
            }
            // TODO: Close the SQL connection (or is this done automatically?)
            SubmitDBChanges();
            database.Connection.Close();
            // I moved the Join here for now, since this hangs waiting for a tag
            //     read to unstick the network TcpListener.AcceptTcpClient.
            // It should probably in the EndRace() method.
            if (raceStarted)
            {
                racingThread.Join();
            }
        }

        private void EndRace()
        {
            listener.End();
            passDetector.Exit();
        }
        #endregion

        private void summaryButton_Click(object sender, EventArgs e)
        {
            var data = from reading in database.TagReadings
                       group reading by reading.Tag into tags
                       select new { Tag = tags.Key, Count = tags.Count() };
            
            var tagSummary = from tag in data orderby tag.Tag select tag;

            statusTextBox.AppendText("Summary of Tag Readings (total):");
            statusTextBox.AppendText(Environment.NewLine);
            foreach (var record in tagSummary)
            {
                statusTextBox.AppendText("Tag: ");
                statusTextBox.AppendText(record.Tag);
                statusTextBox.AppendText(",  Count: ");
                statusTextBox.AppendText(record.Count.ToString());
                statusTextBox.AppendText(Environment.NewLine);
            }
        }
    }
}
