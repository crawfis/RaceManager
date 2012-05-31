using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using HardCard.Scoring;
using OhioState.Collections;
using System.Threading;



namespace SampleGUI
{
    public partial class Form1 : Form
    {
        private LoggerSql readingsLogger;
        private LoggerSql passingsLogger;
        SqlConnection sqlConnection;
        private NetworkListener listener;
        private ProcessBufferedReadings passDetector;
        private Thread racingThread;
        private bool raceStarted = false;
        SqlDataAdapter sqlAdapter;
        DataTable table;
        BindingSource bindingSource;
        private DataClasses1DataContext database = new DataClasses1DataContext();

        public Form1()
        {
            InitializeComponent();
            SetupLogger();
            timer1.Start();
        }

        private void SetupLogger()
        {
            // Open connection
            sqlConnection = new SqlConnection(
                Properties.Settings.Default.HardcardTestDatabaseConnectionString);

            sqlConnection.Open();
            // Create new DataAdapter
            //sqlAdaptor = new SqlDataAdapter(
            //    "SELECT * FROM TagReadings", sqlConnection);

            // Use DataAdapter to fill DataTable
            //table = new DataTable();
            //sqlAdaptor.FillSchema(table, SchemaType.Mapped);
            //sqlAdaptor.Fill(table);
            //// Render data onto the screen
            ////BindingSource to sync DataTable and DataGridView
            BindingSource bindingSource = new BindingSource();

            ////set the BindingSource DataSource
            //bindingSource.DataSource = table;

            //// set the DataGridView DataSource
            //this.readingsView.DataSource = bindingSource;

            readingsLogger = new LoggerSql(sqlConnection, LogReadings);
            passingsLogger = new LoggerSql(sqlConnection, LogPassings);
        }

        private int readingsCount = 0;
        private void LogReadings(TagReadEventArgs e)
        {
            TagInfo tagInfo = e.TagInfo;
            TagReading reading = new TagReading();
            reading.ReadingNumber = readingsCount;
            reading.Antenna = (short) tagInfo.Antenna;
            reading.Frequency = tagInfo.Frequency;
            reading.SignalStrength = tagInfo.SignalStrenth;
            reading.Tag = tagInfo.ID.Value;
            reading.Time = tagInfo.Time;
            database.TagReadings.InsertOnSubmit(reading);
            database.SubmitChanges();
            readingsCount++;
            //hardcardTestDatabaseDataSet.TagReadings.AddTagReadingsRow(tagInfo.ID.Value, tagInfo.Time, tagInfo.SignalStrenth, (short)tagInfo.Antenna, tagInfo.Frequency);

        }

        private void LogPassings(TagReadEventArgs e)
        {
            TagInfo tagInfo = e.TagInfo;
            Passing passing = new Passing();
            passing.Tag = tagInfo.ID.Value;
            passing.PassingTime = tagInfo.Time;
            passing.ReadingNumber = -1;
            passing.Lap = -1;
            //database.Passings.InsertOnSubmit(passing);
            //database.SubmitChanges();

        }

        private void StartRace()
        {
            // TODO: This should be put in a logger
            errorScreen.AppendText(Environment.NewLine);
            errorScreen.AppendText("Race Started at: ");
            errorScreen.AppendText(DateTime.Now.ToLongTimeString());
            errorScreen.AppendText(", ");
            errorScreen.AppendText(DateTime.Now.ToLongDateString());
            errorScreen.AppendText(Environment.NewLine);

            racingThread = new Thread(StartRaceThread);
            racingThread.Start();
        }

        private void StartRaceThread()
        {
            listener = new NetworkListener("Race Host");
            int initialiCapacity = 10000;
            PriorityCollectionBlocking<TagInfo> queue = new PriorityCollectionBlocking<TagInfo>("Queue", initialiCapacity);
            BufferReadings buffer = new BufferReadings(queue);
            passDetector = new ProcessBufferedReadings("Pass Detector", queue);
            readingsLogger.AddPublisher(listener);
            buffer.AddPublisher(listener);
            passingsLogger.AddPublisher(passDetector);
            passDetector.Start();
            listener.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (raceStarted)
            {
                listener.End();
                passDetector.Exit();
                racingThread.Join();
            }
            sqlConnection.Close();

            // TODO: This should be put in a logger
            // This will never be seen since the form is closing.
            //errorScreen.AppendText(Environment.NewLine);
            //errorScreen.AppendText("Race Ended at: ");
            //errorScreen.AppendText(DateTime.Now.ToLongTimeString());
            //errorScreen.AppendText(", ");
            //errorScreen.AppendText(DateTime.Now.ToLongDateString());
            //errorScreen.AppendText(Environment.NewLine);
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            StartRace();
            raceStarted = true;
            startButton.Text = "Running . . .";
            startButton.Enabled = false;
        }

        private int index = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            index++;
            index = index % (imageList1.Images.Count);
            this.pictureBox1.Image = this.imageList1.Images[index];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Create a new data adapter based on the specified query.
            //sqlAdapter = new SqlDataAdapter("SELECT * FROM TagReadings",
            //    Properties.Settings.Default.HardcardTestDatabaseConnectionString);

            //// Create a command builder to generate SQL update, insert, and
            //// delete commands based on selectCommand. These are used to
            //// update the database.
            //SqlCommandBuilder commandBuilder = new SqlCommandBuilder(sqlAdapter);

            //// Populate a new data table and bind it to the BindingSource.
            //DataTable table = new DataTable();
            //table.Locale = System.Globalization.CultureInfo.InvariantCulture;
            //sqlAdapter.Fill(table);
            //this.hardcardTestDatabaseDataSetBindingSource.DataSource = table;

            //// Resize the DataGridView columns to fit the newly loaded content.
            //readingsView.AutoResizeColumns(
            //    DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);


            bool errors = this.hardcardTestDatabaseDataSet.HasErrors;
            this.hardcardTestDatabaseDataSet.TagReadings.AcceptChanges();
            this.readingsView.Invalidate();
            this.readingsView.Update();
            SqlCommand command = new SqlCommand("SELECT * FROM TagReadings", sqlConnection);
            SqlDataReader reader = command.ExecuteReader();
            int count = 0;
            foreach (var row in reader)
            {
                string s = row.ToString();
                if (s != null)
                {
                    int nothing = count;
                }
            }
            reader.Close();
            command.Cancel();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'hardcardTestDatabaseDataSet.TagReadings' table. You can move, or remove it, as needed.
            this.tagReadingsTableAdapter.Fill(this.hardcardTestDatabaseDataSet.TagReadings);
            // This line loads data into the Passings
            //passingBindingSource.DataSource = from passing in database.Passings
            //                                  orderby passing.PassingNumber
            //                                  select passing;

        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Validate();
            passingBindingSource.EndEdit();
            database.SubmitChanges();
        }
    }
}
