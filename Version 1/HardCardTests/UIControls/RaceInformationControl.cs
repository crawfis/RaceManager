#define V15

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EventProject;
using Hardcard.Scoring;
using OhioState.Collections;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Collections;

namespace UIControls
{
    public partial class RaceInformationControl : UserControl
    {
        //private BindingList<CompetitorRace> currentRaceList;
        private SortableBindingList<CompetitorRace> currentRaceList;
        private Race currentRace;
        private Hashtable raceCompetitorsHash;
        private bool passingsGridScrolled;
        private System.Windows.Forms.Timer standingsTimer = new System.Windows.Forms.Timer();

        public Hashtable RaceCompetitorsHash
        {
            get { return raceCompetitorsHash; }
            set { raceCompetitorsHash = value; }
        }

        public Event GetSelectedEvent()
        {
            if (eventComboBox.SelectedValue != null)
                return eventComboBox.SelectedValue as Event;
            
            return null;
        }

        public RaceInformationControl()
        {
            InitializeComponent();

            this.Dock = DockStyle.Fill;
            this.stopCollectingDataButton.Enabled = false;
            this.selectedRaceComboBox.Enabled = true;
            this.eventComboBox.Enabled = true;

            eventComboBox.DataSource = DataManager.Instance.Events;
            eventComboBox.SelectedIndexChanged += new EventHandler(eventComboBox_SelectedIndexChanged);
            eventComboBox.VisibleChanged += new EventHandler(eventComboBox_VisibleChanged);

            currentRaceList = new SortableBindingList<CompetitorRace>();//new BindingList<CompetitorRace>();
            //currentRaceList = new BindingList<CompetitorRace>();
            raceCompetitorsHash = new Hashtable();
            
            startCollectingDataButton.Click += new EventHandler(startCollectingDataButton_Click);
            stopCollectingDataButton.Click += new EventHandler(stopCollectingDataButton_Click);

            selectedRaceComboBox.SelectedIndexChanged += new EventHandler(selectedRaceComboBox_SelectedIndexChanged);
            selectedRaceComboBox.VisibleChanged += new EventHandler(selectedRaceComboBox_VisibleChanged);

            passingsDataGrid.DataSource = new BindingList<PassingsInfo>();
            passingsDataGrid.MultiSelect = false;
            passingsDataGrid.Columns["CompetitorRace"].Visible = false;
            passingsDataGrid.Columns["Frequency"].Visible = false;
            passingsDataGrid.Columns["SignalStrength"].Visible = false;
            passingsDataGrid.MouseDoubleClick += new MouseEventHandler(passingsDataGrid_MouseDoubleClick);
            passingsDataGrid.Scroll += new ScrollEventHandler(passingsDataGrid_Scroll);

            exportPassingsButton.Click += new EventHandler(exportPassingsButton_Click);
            exportRaceInfoButton.Click += new EventHandler(exportRaceInfoButton_Click);

            addPassingButton.Click += new EventHandler(addPassingButton_Click);
            removePassingButton.Click += new EventHandler(removePassingButton_Click);
            undeletePassingButton.Click += new EventHandler(undeletePassingButton_Click);

            enableAutoScrollButton.Click += new EventHandler(enableAutoScrollButton_Click);
            disableAutoscrollButton.Click += new EventHandler(disableAutoscrollButton_Click);

            sortByClassCheckBox.Click += new EventHandler(sortByClassCheckBox_Click);
            firstPassingLapOneCheckbox.Click += new EventHandler(sortByClassCheckBox_Click);

            standingsTimer.Interval = 10000;
            standingsTimer.Tick += new EventHandler(standingsTimer_Tick);
            standingsTimer.Start(); 
        }

        public void Close()
        {
            if (stopCollectingDataButton.Enabled)
            {
                this.stopCollectingDataButton_Click(null, null);
            }
        }

        private void sortByClassCheckBox_Click(object sender, EventArgs e)
        {
            UpdateRaceStandingsGrid(null);
        }

        private void disableAutoscrollButton_Click(object sender, EventArgs e)
        {
            passingsGridScrolled = true;            
        }

        private void enableAutoScrollButton_Click(object sender, EventArgs e)
        {
            passingsGridScrolled = false;
        }

        private void undeletePassingButton_Click(object sender, EventArgs e)
        {
            if (passingsDataGrid.SelectedRows.Count == 0)
                return;

            if (MessageBox.Show("Are you sure you would like to undelete this tag?",
                "Please confirm tag undeletion.", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            int index = passingsDataGrid.SelectedRows[0].Index;
            PassingsInfo pi = (passingsDataGrid.DataSource as BindingList<PassingsInfo>)[index];
            pi.Deleted = "";

            UpdateRaceStandingsGrid(null);
        }

        private void removePassingButton_Click(object sender, EventArgs e)
        {
            if (passingsDataGrid.SelectedRows.Count == 0)
                return;

            if (MessageBox.Show("Are you sure you would like to delete this tag?",
                "Please confirm tag deletion.", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            int index = passingsDataGrid.SelectedRows[0].Index;
            PassingsInfo pi = (passingsDataGrid.DataSource as BindingList<PassingsInfo>)[index];
            pi.LapTime = "-1";
            pi.Deleted = "DELETED";

            UpdateRaceStandingsGrid(null);
        }

        private void addPassingButton_Click(object sender, EventArgs e)
        {
            long existingTime = 1;
            int index = -1;
            if (passingsDataGrid.SelectedRows.Count != 0)
            {
                index = passingsDataGrid.SelectedRows[0].Index;
                PassingsInfo pi = (passingsDataGrid.DataSource as BindingList<PassingsInfo>)[index];
                existingTime = pi.Time + 1;//- 1;
            }
            
            Form addPassingDialog = new Form();
            addPassingDialog.Text = "Add a new passing";
            //addPassingDialog.Width = 300;
            //addPassingDialog.Height = 150;
            Button okButton = new Button();
            Button cancelButton = new Button();
            TextBox tagTextBox = new TextBox();
            Label tagLabel = new Label();
            tagLabel.Text = "TagID";
            okButton.Text = "OK";
            cancelButton.Text = "Cancel";

            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            addPassingDialog.Width = 400;
            addPassingDialog.Height = 135;
            okButton.SetBounds(30, 70, 125, 22);
            cancelButton.SetBounds(160, 70, 120, 22);
            tagLabel.SetBounds(30, 20, 50, 22);
            tagTextBox.SetBounds(85, 20, 196, 22);

            addPassingDialog.Controls.Add(okButton);
            addPassingDialog.Controls.Add(cancelButton);
            addPassingDialog.Controls.Add(tagTextBox);
            addPassingDialog.Controls.Add(tagLabel);
            
            addPassingDialog.CancelButton = cancelButton;

            if (addPassingDialog.ShowDialog() == DialogResult.OK)
            {
                TagId tagID = new TagId(tagTextBox.Text);
                TagInfo ti = new TagInfo(tagID, 0, 0, 0, existingTime);
                TagReadEventArgs newTagArgs = new TagReadEventArgs(TagEventType.NewTagDetected, ti);
                if (index != -1) index += 1;
                UpdatePassingsGrid(newTagArgs, index);
                UpdateRaceStandingsGrid(newTagArgs);
            }

            addPassingDialog.Dispose();
        }

        private void exportPassingsButton_Click(object sender, EventArgs e)
        {
            FileDialog dialog = new SaveFileDialog();
            dialog.Title = "Export Passings";
            dialog.Filter = "CSV files (*.csv)|*.csv |html files (*.html)|*.html";
            if (dialog.ShowDialog() == DialogResult.Cancel)
                return;

            if (dialog.FilterIndex == 1)
                //ExportPassingsToFileCSV(dialog.FileName);
                DataManager.Instance.ExportPassingsToFileCSV(dialog.FileName, (passingsDataGrid.DataSource as BindingList<PassingsInfo>));
            else if (dialog.FilterIndex == 2)
                //ExportPassingsToFileHTML(dialog.FileName);
                DataManager.Instance.ExportPassingsToFileHTML(dialog.FileName, (passingsDataGrid.DataSource as BindingList<PassingsInfo>));
        }

        private void exportRaceInfoButton_Click(object sender, EventArgs e)
        {
            FileDialog dialog = new SaveFileDialog();
            dialog.Title = "Export Race Information";
            dialog.Filter = "CSV files (*.csv)|*.csv |html files (*.html)|*.html";
            if (dialog.ShowDialog() == DialogResult.Cancel)
                return;

            if (dialog.FilterIndex == 1)
                //ExportRaceInfoFileCSV(dialog.FileName);
                DataManager.Instance.ExportRaceInfoFileCSV(dialog.FileName, currentRaceList);
            else if (dialog.FilterIndex == 2)
                //ExportRaceInfoFileHTML(dialog.FileName);
                DataManager.Instance.ExportRaceInfoFileHTML(dialog.FileName, currentRaceList);
        }

        private void eventComboBox_VisibleChanged(object sender, EventArgs e)
        {
            if (!eventComboBox.Enabled) return;//don't change selection during race
            
            if (eventComboBox.SelectedIndex != -1)
            {
                selectedRaceComboBox.DataSource = DataManager.Instance.Events[eventComboBox.SelectedIndex].races;

                if (DataManager.Instance.SessionSelectedIndex != -1 &&
                    (selectedRaceComboBox.DataSource as BindingList<Race>).Count > DataManager.Instance.SessionSelectedIndex)
                {
                    selectedRaceComboBox.SelectedIndex = DataManager.Instance.SessionSelectedIndex;
                }
            }

            ResetControls();
        }

        private void eventComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (eventComboBox.SelectedIndex != -1)
            {
                int sessionIndex = DataManager.Instance.SessionSelectedIndex;//trick!
                //have to use sessionIndex variable, as the next line resets it to 0 (only when called first time!)
                selectedRaceComboBox.DataSource = DataManager.Instance.Events[eventComboBox.SelectedIndex].races;
                DataManager.Instance.EventSelectedIndex = eventComboBox.SelectedIndex;

                if (sessionIndex != -1 && (selectedRaceComboBox.DataSource as BindingList<Race>).Count > sessionIndex)
                {
                    selectedRaceComboBox.SelectedIndex = sessionIndex;
                }
                //reset back!
                DataManager.Instance.SessionSelectedIndex = sessionIndex;
            }

            ResetControls();
        }

        private void selectedRaceComboBox_VisibleChanged(object sender, EventArgs e)
        {
            if (!eventComboBox.Enabled) return;//don't change selection during race
            
            if (DataManager.Instance.EventSelectedIndex != -1 &&
                DataManager.Instance.Events.Count > DataManager.Instance.EventSelectedIndex)
            {
                eventComboBox.SelectedIndex = DataManager.Instance.EventSelectedIndex;
            }
        }

        private void selectedRaceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataManager.Instance.SessionSelectedIndex = selectedRaceComboBox.SelectedIndex;
            DataManager.Instance.EventSelectedIndex = eventComboBox.SelectedIndex;

            ResetControls();
        }

        public Race GetCurrentRace()
        {
            return currentRace;
        }

        /// <summary>
        /// This method resets the controls once the user changes selection in the 
        /// events or races combo boxes.
        /// </summary>
        private void ResetControls()
        {
            int selectedRaceIndex = selectedRaceComboBox.SelectedIndex;
            if (selectedRaceIndex == -1)
            {
                passingsDataGrid.DataSource = new BindingList<PassingsInfo>();
                passingsDataGrid.Columns["CompetitorRace"].Visible = false;
                passingsDataGrid.Columns["DateTime"].DefaultCellStyle.Format = "MM/dd/yyyy HH:mm:ss.fff";

                raceInformationGridView.DataSource = new BindingList<Race>();
                //raceInformationGridView.Columns["competitor"].Visible = false;
                if (currentRaceList != null)
                    currentRaceList.ListChanged -= currentRaceList_ListChanged;
                currentRaceList = new SortableBindingList<CompetitorRace>();//new BindingList<CompetitorRace>();
                //currentRaceList = new BindingList<CompetitorRace>();
                currentRaceList.ListChanged += currentRaceList_ListChanged;

                currentRace = null;

                return;
            }

            //whenever competitorrace is removed from a race (racelist), make sure corresponding PI's are stored at
            //the race object - add this functionality to race class
            //basically add methods CompetitorRaceAdd, CompetitorRaceRemove to the Race class;
            //they should do standard adding/removal, but also take care of the passings

            //race keeps passings that were not associated with any competitor
            currentRace = (selectedRaceComboBox.DataSource as BindingList<Race>)[selectedRaceIndex];
            //add non-associated passings to current passings list

            try
            {
                if(currentRaceList != null)
                    currentRaceList.ListChanged -= currentRaceList_ListChanged;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                DataManager.Log("Exception Caught!" + exc.StackTrace);
            }//exception should never happen

            currentRaceList = //(selectedRaceComboBox.DataSource as BindingList<Race>)[selectedRaceIndex].competitorRaceList;
                (selectedRaceComboBox.DataSource as BindingList<Race>)[selectedRaceIndex].competitorRaceList;
            currentRaceList.ListChanged += currentRaceList_ListChanged;

            raceInformationGridView.DataSource = currentRaceList;
            raceInformationGridView.Columns["competitor"].Visible = false;
            raceInformationGridView.Columns["competitorID"].Visible = false;
            raceInformationGridView.Columns["raceParent"].Visible = false;
            raceInformationGridView.Columns["EventEntry"].Visible = false;
            raceInformationGridView.Columns["BestLap"].Visible = false;
            raceInformationGridView.Columns["BestLap"].Width = 130;
            raceInformationGridView.Columns["LastLap"].Visible = false;
            raceInformationGridView.Columns["LastLap"].Width = 130;
            raceInformationGridView.Columns["Position"].DisplayIndex = 0;
            raceInformationGridView.Columns["Position"].Width = 45;
            raceInformationGridView.Columns["tagID"].Width = 40;
            raceInformationGridView.Columns["tagID2"].Width = 40;
            raceInformationGridView.Columns["bikeNumber"].Width = 40;
            raceInformationGridView.Columns["lapsCompleted"].Width = 40;

            passingsDataGrid.DataSource = new BindingList<PassingsInfo>();
            passingsDataGrid.Columns["CompetitorRace"].Visible = false;
            passingsDataGrid.Columns["competitorID"].Visible = false;
            passingsDataGrid.Columns["DateTime"].DefaultCellStyle.Format = "HH:mm:ss.fff"; //"MM/dd/yyyy HH:mm:ss.fff";
            //passingsDataGrid.Columns["firstName"].Visible = false;
            //passingsDataGrid.Columns["lastName"].Visible = false;
            passingsDataGrid.Columns["Time"].Visible = false;
            passingsDataGrid.Columns["ID"].Width = 50;
            passingsDataGrid.Columns["Antenna"].Width = 50;
            passingsDataGrid.Columns["Hits"].Width = 50;
            passingsDataGrid.Columns["CompetitionNumber"].Width = 70;
            
            //first add passings to a list and sort;
            //only then add them to binding list
            //have to do this, otherwise passings will be "sorted"
            //by competitor
            List<PassingsInfo> passingsList = new List<PassingsInfo>();

            foreach (CompetitorRace cr in currentRaceList)
            {
                foreach (PassingsInfo pi in cr.passings)
                {
                    if(!passingsList.Contains(pi))
                        passingsList.Add(pi);
                }
            }

            if (currentRace != null)
            {
                foreach (PassingsInfo pi in currentRace.passings)
                {
                    if (!passingsList.Contains(pi))
                        passingsList.Add(pi);
                }
            }

            //sort passings list by timestamp
            passingsList.Sort(delegate(PassingsInfo p1, PassingsInfo p2)
            {
                if (p1.Time < p2.Time) return -1;
                else if (p1.Time > p2.Time) return 1;
                else return 0;
            });

            foreach (PassingsInfo pi in passingsList)
            {
                if (!(passingsDataGrid.DataSource as BindingList<PassingsInfo>).Contains(pi))
                    (passingsDataGrid.DataSource as BindingList<PassingsInfo>).Add(pi);
            }

            UpdateRaceStandingsGrid(null);

            if (currentRace != null)
            {
                tagClashFormList = currentRace.tagClashFormList;
                disambiuationCRDict = currentRace.disambiuationCRDict;
            }
        }

        //
        //this listener is necessary to capture updates to the race list, which
        //may happen through the race (new people added on the fly, some people 
        //removed from the race, tags changed, etc)
        //
        private void currentRaceList_ListChanged(object sender, ListChangedEventArgs e)
        {
            //try
            //{
            //    CalculateRaceStandings();
            //}
            //catch (Exception exc)
            //{
            //    Console.WriteLine(exc.StackTrace);
            //    DataManager.Log("Exception Caught!" + exc.StackTrace);
            //}

            try
            {
                //have to remove listener, otherwise we'll get a stack overflow 
                //due to recursive calls
                if (currentRaceList != null)
                    currentRaceList.ListChanged -= currentRaceList_ListChanged;

                UpdateRaceStandingsGrid(null);

                if (currentRaceList != null)
                    currentRaceList.ListChanged += currentRaceList_ListChanged;

            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.StackTrace);
                DataManager.Log("Exception Caught!" + exc.StackTrace);
            }

            return; //all else is handled in Race / CompetitorRace classes now
        }

        private NetworkListener networkListener;
        private ProcessBufferedReadings passDetector;
        private TagSubscriber readingsLogger;
        private TagSubscriber passingsLogger;
        private LoggerBinary binaryLogger;
        private IPriorityCollection<TagInfo> readingsQueue;
        private IPriorityCollection<TagInfo> passingsQueue;

        public void SetSettings(int portNumber, int sleepTime)
        {
            //Config.Instance.SleepTime = sleepTime;
            //Config.Instance.PortNumber = portNumber;

            //MessageBox.Show("SleepTime is: " + sleepTime + "; PortNumber is: " + portNumber);
        }
        private HardcardServer server;
        private NetworkPublisher networkFeed;
        private void startCollectingDataButton_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("SleepTime is: " + Config.Instance.SleepTime + "; PortNumber is: " + Config.Instance.PortNumber);
            
            startCollectingDataButton.Enabled = false;
            stopCollectingDataButton.Enabled = true;
            //so that user won't switch during the event:
            this.selectedRaceComboBox.Enabled = false;
            this.eventComboBox.Enabled = false;
            ResetRaceStandingsGrid();
            
            //passingsDataGrid.DataSource = new BindingList<PassingsInfo>();
            
            readingsLogger = new TagSubscriber(LogReadings);
            passingsLogger = new TagSubscriber(LogPassings);
                        
            readingsQueue = new PriorityCollectionNonBlocking<TagInfo>("Queue", 1024);
            passingsQueue = new PriorityCollectionNonBlocking<TagInfo>("Queue", 1024);

#if V15
            HardcardServer.NetworkPort = Properties.Settings.Default.PortNumber;
            HardcardServer.PassingSleepTime = 1000;
            server = new HardcardServer("Hardcard Race System");
            //LoggerConsole log = new LoggerConsole();
            //LoggerSummary countLogger = new LoggerSummary();
            //LoggerBinary binaryLog = new LoggerBinary("tagReadings.bin");
            //binaryLog.AddPublisher(server.IndividualReadingPublisher);
            //countLogger.AddPublisher(server.IndividualReadingPublisher);
            //countLogger.DesiredTagTypes = TagEventType.Read;

            readingsLogger.AddPublisher(server.PassingsPublisher);
            readingsLogger.DesiredTagTypes = TagEventType.PassDetermined;
            passingsLogger.AddPublisher(server.PassingsPublisher);
            passingsLogger.DesiredTagTypes = TagEventType.PassDetermined;

            bool useXML = Properties.Settings.Default.useXML;
            networkFeed = new NetworkPublisher(useXML);
            networkFeed.Server = Properties.Settings.Default.serverIP;
            networkFeed.Port = Properties.Settings.Default.serverPortNumber;
            networkFeed.AddPublisher(server.PassingsPublisher);
            networkFeed.DesiredTagTypes = TagEventType.PassDetermined;
            networkFeed.Start();
            server.Start();

#else
            int portNumber = Properties.Settings.Default.PortNumber;
            networkListener = new NetworkListener("Race Host",portNumber);
            int initialCapacity = 10000;
            PriorityCollectionBlocking<TagInfo> queue = new PriorityCollectionBlocking<TagInfo>("Queue", initialCapacity);
            // The queue above is used to communicate between the buffer below and the passDetector.
            BufferReadings buffer = new BufferReadings(queue);
            passDetector = new ProcessBufferedReadings("Pass Detector", queue);
            buffer.AddPublisher(networkListener);

            // Subscribe the loggers to the publishers.
            readingsLogger.AddPublisher(networkListener);
            passingsLogger.AddPublisher(passDetector);
            passDetector.Start();
            networkListener.Start();
#endif
        }

        private void CreateBinLogger()
        {
            String binLogFileName = (currentRace != null) ? GetCleanedRaceName(currentRace.name) + "_out.bin" : "_out.bin";
            string appPath = Path.GetDirectoryName(Config.Instance.DataDirectory);
            Directory.SetCurrentDirectory(appPath);
            if (!Directory.Exists("runs"))
                Directory.CreateDirectory("runs");
            Directory.SetCurrentDirectory(appPath + "\\runs");

            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    if (eventComboBox.SelectedIndex != -1)
                        binLogFileName = DataManager.Instance.Events[eventComboBox.SelectedIndex].name + "_" + binLogFileName;
                });
            }
            else
            {
                if (eventComboBox.SelectedIndex != -1)
                    binLogFileName = DataManager.Instance.Events[eventComboBox.SelectedIndex].name + "_" + binLogFileName;
            }

            //if file exists, append some counter to it
            int fileNameCounter = 1;
            if (File.Exists(binLogFileName))
            {
                String currentFileName = binLogFileName;
                while (File.Exists(currentFileName))
                {
                    int strIndex = currentFileName.IndexOf(".bin");
                    if (strIndex > 0)
                    {
                        currentFileName = currentFileName.Remove(strIndex - 1, 1);
                        currentFileName = currentFileName.Insert(strIndex - 1, "" + fileNameCounter);
                    }
                    fileNameCounter++;
                    if (fileNameCounter > 10)
                        break;
                }
                binLogFileName = currentFileName;
            }
            binaryLogger = new LoggerBinary(binLogFileName);
#if V15
            binaryLogger.AddPublisher(server.IndividualReadingPublisher);
#else
            binaryLogger.AddPublisher(networkListener);
#endif
        }

        private String GetCleanedRaceName(String raceName)
        {
            if(raceName == null) return "";

            String newRaceName = raceName.Replace('/', '_');
            newRaceName = newRaceName.Replace('\\', '_');

            return newRaceName;
        }

        private void stopCollectingDataButton_Click(object sender, EventArgs e)
        {
            startCollectingDataButton.Enabled = true;
            stopCollectingDataButton.Enabled = false;
            selectedRaceComboBox.Enabled = true;
            this.eventComboBox.Enabled = true;
#if V15
            networkFeed.Quit();
            server.End();
#else
            networkListener.End();
            passDetector.Exit();
#endif
            if(binaryLogger != null)
                binaryLogger.Dispose();
            binaryLogger = null;
        }

        private void LogReadings(TagReadEventArgs e)
        {
            readingsQueue.Put(e.TagInfo);
        }
        
        private void LogPassings(TagReadEventArgs e)
        {
            if (binaryLogger == null)
            {
                try
                {
                    CreateBinLogger();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    DataManager.Log(ex.StackTrace);
                }
            }

#if !V15
            if (DataManager.Instance.ValidTags.ContainsKey(e.TagInfo.ID.Value))
            {
                DataManager.Instance.MarkTagAsUsed(e.TagInfo.ID);
#endif
                passingsQueue.Put(e.TagInfo);

                this.SuspendLayout();
                
                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        UpdatePassingsGrid(e);
                    });
                }
                else
                {
                    UpdatePassingsGrid(e);
                }

                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        UpdateRaceStandingsGrid(e);
                    });
                }
                else
                {
                    UpdateRaceStandingsGrid(e);
                }

                this.ResumeLayout();
            }
#if !V15
        }
#endif

        public void UpdatePassingsGrid(TagReadEventArgs e, int addIndex = -1)
        {
            PassingsInfo pi = null;
            bool tagClash = false;
            CompetitorRace cr = null;
            List<CompetitorRace> crs = FindCompetitorRaceByTag(e.TagInfo.ID);
            if (crs.Count == 1)
                cr = crs[0];
            else if(crs.Count > 1)
                tagClash = true;
            
            EventEntry eve = null;
            if (cr == null && !tagClash)
            {
                eve = isCompetitorInEventThreadSafe(e.TagInfo.ID);

                //the following may happen - for example, event entry has IDs of removed competitor;
                //then we should go as if there is not corresponding event entry
                if (eve != null)
                {
                    if (DataManager.Instance.GetCompetitorByID(eve.competitorID) == null)
                        eve = null;
                }
            }

            if (cr != null)
            {
                pi = new PassingsInfo(e.TagInfo, cr.competitorID, cr.firstName, cr.lastName, cr.bikeNumber);
                pi.CompetitorRace = cr;
                if (addIndex == -1 || addIndex >= cr.passings.Count)
                    cr.passings.Add(pi);
                else
                    cr.passings.Insert(addIndex, pi);
            }
            else if (eve != null)
            {                
                CompetitorRace crNew = new CompetitorRace(eve.competitorID);
                //can competitor be null? probably not, as EventEntry is not null
                crNew.competitor = DataManager.Instance.GetCompetitorByID(eve.competitorID);
                crNew.EventEntry = eve;
                crNew.lastName = (crNew.competitor != null) ? crNew.competitor.LastName : "";
                crNew.firstName = (crNew.competitor != null) ? crNew.competitor.FirstName : "";
                crNew.competitorID = eve.competitorID;
                crNew.tagID = eve.tagNumber;
                crNew.tagID2 = eve.tagNumber2;
                crNew.bikeNumber = eve.bikeNumber;
                crNew.raceParent = this.GetCurrentSession();
                if (currentRace != null)//should never happen
                {
                    currentRace.competitorRaceList.Add(crNew);
                    crNew.raceParent = currentRace;
                }
                pi = new PassingsInfo(e.TagInfo, crNew.competitorID, crNew.firstName, crNew.lastName, crNew.bikeNumber);
                pi.CompetitorRace = cr;//crNew???
                if (addIndex == -1 || addIndex >= cr.passings.Count || addIndex >= crNew.passings.Count)//
                    crNew.passings.Add(pi);
                else
                    crNew.passings.Insert(addIndex, pi);
            }
            else
            {
                //have to create a new CompetitorRace:
                cr = new CompetitorRace();
                cr.isNull = true;
                //make its competitor's ID negative:
                //this will prevent possible clash with real
                //competitors' IDs
                cr.competitorID = -1 * DataManager.getNextID();
                cr.tagID = e.TagInfo.ID;
                cr.tagID2 = e.TagInfo.ID;
                cr.firstName = "";
                cr.lastName = "Unassigned";
                if (currentRace != null)//should never happen
                {
                    //currentRace.competitorRaceList is the currentRaceList
                    currentRace.competitorRaceList.Add(cr);
                    cr.raceParent = currentRace;
                }

                pi = new PassingsInfo(e.TagInfo, cr.competitorID, cr.firstName, cr.lastName, cr.bikeNumber);
                pi.CompetitorRace = cr;
                if (addIndex == -1 || addIndex >= cr.passings.Count)
                    cr.passings.Add(pi);
                else
                    cr.passings.Insert(addIndex, pi);
            }


            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    if (addIndex == -1 || addIndex >= cr.passings.Count || addIndex >= (passingsDataGrid.DataSource as BindingList<PassingsInfo>).Count)
                        (passingsDataGrid.DataSource as BindingList<PassingsInfo>).Add(pi);
                    else
                        (passingsDataGrid.DataSource as BindingList<PassingsInfo>).Insert(addIndex, pi);

                    if (passingsDataGrid.RowCount > 0 && !passingsGridScrolled)
                    {
                        passingsDataGrid.Rows[passingsDataGrid.RowCount - 1].Selected = true;
                        passingsDataGrid.CurrentCell = passingsDataGrid.Rows[passingsDataGrid.RowCount - 1].Cells[0];
                    }
                });
            }
            else
            {
                if (addIndex == -1 || addIndex >= cr.passings.Count || addIndex >= (passingsDataGrid.DataSource as BindingList<PassingsInfo>).Count)
                    (passingsDataGrid.DataSource as BindingList<PassingsInfo>).Add(pi);
                else
                    (passingsDataGrid.DataSource as BindingList<PassingsInfo>).Insert(addIndex, pi);

                if (passingsDataGrid.RowCount > 0 && !passingsGridScrolled)
                {
                    passingsDataGrid.Rows[passingsDataGrid.RowCount - 1].Selected = true;
                    passingsDataGrid.CurrentCell = passingsDataGrid.Rows[passingsDataGrid.RowCount - 1].Cells[0];
                }
            }

            if (tagClash)
            {
                //TODO: after the user has selected the Competitor, this should remain for the whole
                //run; need some kind of Dictionary for that; it should be looked up first in
                //the method findCRbyTag;
                //also, don't forget to reset everything for a new run; for instance, tagClashFormList

                if (!tagClashFormList.Contains(pi.ID))
                {
                    List<CompetitorRace> crsNew = new List<CompetitorRace>();
                    foreach (CompetitorRace cRace in crs)
                    {
                        if (cRace.competitorID >= 0)//don't need "Unassigned" CRs
                            crsNew.Add(cRace);
                    }
                    if (crsNew.Count > 1)
                    {
                        tagClashFormList.Add(pi.ID);
                        TagClashForm clashForm = new TagClashForm();
                        clashForm.SetCompetitors(crsNew, pi, tagClashFormList, currentRace, passingsDataGrid, disambiuationCRDict);
                        clashForm.Show(this);
                    }
                }
            }

            //check if pi.lapTime is smaller than min lap time, make it "DELETED" if true
            if (pi != null && pi.CompetitorRace != null)
            {
                if (pi.CompetitorRace.passings != null && pi.CompetitorRace.passings.Count > 1)
                {
                    PassingsInfo piPrevious = null;
                    //look for any previous valid passings
                    for (int i = pi.CompetitorRace.passings.Count - 2; i >= 0; i--)
                    {
                        if (!pi.CompetitorRace.passings[i].Deleted.Equals("DELETED"))
                        {
                            piPrevious = pi.CompetitorRace.passings[i];
                            break;
                        }
                    }
                    if (piPrevious != null)
                    {
                        long latestTime = pi.Time;
                        long nextToLatestTime = piPrevious.Time;
                        double lapTime = (latestTime - nextToLatestTime) / 1000.0;
                        if (lapTime < DataManager.Instance.MinimumLapTime)
                        {
                            pi.Deleted = "DELETED";
                            pi.LapTime = GetTimeString(lapTime);
                        }
                    }
                }
            }
        }

        private String GetTimeString(double time)//time is in seconds
        {            
            int hours = (int)Math.Floor(time / 3600.0);
            int minutes = (int)Math.Floor((time - hours * 3600) / 60.0);
            double seconds = (time - minutes * 60 - hours * 3600);

            String hoursStr = String.Format("{0:00}", hours);
            String minutesStr = String.Format("{0:00}", minutes);
            String secondsStr = String.Format("{0:00.000}", seconds);

            return hoursStr + ":" + minutesStr + ":" + secondsStr;
        }

        private List<TagId> tagClashFormList = new List<TagId>();
        private Dictionary<TagId, CompetitorRace> disambiuationCRDict = new Dictionary<TagId, CompetitorRace>();

        private bool standingsAreDirty = false;
        public void UpdateRaceStandingsGrid(TagReadEventArgs e)
        {
            standingsAreDirty = true;
        }

        void standingsTimer_Tick(object sender, EventArgs e)
        {
            if (!standingsAreDirty)
                return;
            standingsAreDirty = false;

            CalculateRaceStandings(firstPassingLapOneCheckbox.Checked);

            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    //this.raceInformationGridView.Sort(this.raceInformationGridView.Columns["bestLap"], ListSortDirection.Ascending);
                    //Practice
                    //Qualifying
                    //Race
                    //probably better to pass sort method as a param to the ExplicitlySort; otherwise, implementation is too hacky
                    if (currentRace != null && (currentRace.type.Equals("Qualifying") || currentRace.type.Equals("Practice")))
                    {
                        (this.raceInformationGridView.DataSource as SortableBindingList<CompetitorRace>).ExplicitlySort(true, sortByClassCheckBox.Checked);
                    }
                    else if (currentRace != null && currentRace.type.Equals("Race"))
                    {
                        (this.raceInformationGridView.DataSource as SortableBindingList<CompetitorRace>).ExplicitlySort(false, sortByClassCheckBox.Checked);
                    }

                    currentRaceList.ResetBindings();
                });
            }
            else
            {
                if (currentRace != null && (currentRace.type.Equals("Qualifying") || currentRace.type.Equals("Practice")))
                {
                    (this.raceInformationGridView.DataSource as SortableBindingList<CompetitorRace>).ExplicitlySort(true, sortByClassCheckBox.Checked);
                }
                else if (currentRace != null && currentRace.type.Equals("Race"))
                {
                    (this.raceInformationGridView.DataSource as SortableBindingList<CompetitorRace>).ExplicitlySort(false, sortByClassCheckBox.Checked);
                }
                
                currentRaceList.ResetBindings();
            }            
        }

        //ASK: right now best lap is not re-calculated all the time,
        //only if current new lap is better, best lap value is updated
        //reason: to avoid too much of computation
        //should this be changed? Or we can add a button for manual
        //recalculation
        private void CalculateRaceStandings(bool firstPassingIsLapOne = false)
        {
            //new passings should have been added already
            //calculate standings
            foreach (CompetitorRace cr in currentRaceList)
            {
                //sort passings also calculates the best lap
                cr.SortPassings();//comment if it takes too much time to process!

                cr.lastLap = cr.GetLastLapTime();
                if (firstPassingIsLapOne)//means first encountered passing designates lap one
                    cr.lapsCompleted += 1;
            }

            List<CompetitorRace> raceList = new List<CompetitorRace>();
            foreach (CompetitorRace cr in currentRaceList)
            {
                raceList.Add(cr);
            }
            raceList.Sort(delegate(CompetitorRace cr1, CompetitorRace cr2)
            {
                //for standings, check number of laps; if num laps
                //are equal, check who's got the smallest latest timestamp
                if (cr1.lapsCompleted == 0 && cr2.lapsCompleted == 0) return 0;
                if (cr1.lapsCompleted != 0 && cr2.lapsCompleted == 0) return -1;
                if (cr1.lapsCompleted == 0 && cr2.lapsCompleted != 0) return 1;

                if (cr1.lapsCompleted < cr2.lapsCompleted)
                    return 1;
                else if (cr1.lapsCompleted > cr2.lapsCompleted)
                    return -1;
                else //same num laps
                {
                    if (cr1.passings.Count < 1 || cr2.passings.Count < 1) return 0;

                    long cr1Time = cr1.passings[cr1.passings.Count - 1].Time;
                    long cr2Time = cr2.passings[cr2.passings.Count - 1].Time;

                    if (cr1Time < cr2Time)
                        return -1;
                    else if (cr1Time > cr2Time)
                        return 1;
                    return 0;
                }
            });

            if (currentRace != null && currentRace.type.Equals("Race"))
            {
                int position = 1;
                foreach (CompetitorRace cr in raceList)
                {
                    cr.Position = position;
                    position++;
                }
            }
        }

        EditCompetitorDialog editCompetitorDialog = null;
        private void passingsDataGrid_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(passingsDataGrid.SelectedRows.Count == 0)
                return;

            int index = passingsDataGrid.SelectedRows[0].Index;
            PassingsInfo pi = (passingsDataGrid.DataSource as BindingList<PassingsInfo>)[index];

            //fire up a dialog that allows to edit information about the competitor
            editCompetitorDialog = new EditCompetitorDialog();
            editCompetitorDialog.CancelButton = editCompetitorDialog.CancelBtn;
            editCompetitorDialog.SetCompetitorData(pi.competitorID, pi, (passingsDataGrid.DataSource as BindingList<PassingsInfo>), this);

            editCompetitorDialog.ShowDialog();

            editCompetitorDialog.Dispose();
        }

        private void passingsDataGrid_Scroll(object sender, ScrollEventArgs e)
        {
            //passingsGridScrolled = true;
            //Console.WriteLine(sender);
            //Console.WriteLine(e.ToString());
        }

        /// <summary>
        /// Iterate over all entries in the grid and reset positions, 
        /// best/last laps to default values.
        /// </summary>
        private void ResetRaceStandingsGrid()
        {
            if (raceInformationGridView.DataSource as BindingList<CompetitorRace> == null)
                return;

            try
            {
                foreach (CompetitorRace cr in (raceInformationGridView.DataSource as BindingList<CompetitorRace>))
                {
                    cr.bestLap = 0;
                    cr.lastLap = 0;
                    //cr.passings = new BindingList<PassingsInfo>();
                    cr.Position = 0;
                }
            }
            catch (Exception exc)//should never happen
            {
                Console.WriteLine(exc.StackTrace);
                DataManager.Log(exc.StackTrace);
            }
        }

        private List<CompetitorRace> FindCompetitorRaceByTag(TagId tagID)
        {
            List<CompetitorRace> result = new List<CompetitorRace>();
            if (disambiuationCRDict.ContainsKey(tagID))
            {
                if (disambiuationCRDict[tagID].checkTag(tagID))//CRs tag may have changed!
                {
                    result.Add(disambiuationCRDict[tagID]);
                    return result;
                }
            }

            foreach (CompetitorRace cr in currentRaceList)
            {
                if (cr.checkTag(tagID))
                    result.Add(cr);
                    //return cr;
            }
            
            //return null;
            return result;
        }

        private EventEntry isCompetitorInEventThreadSafe(TagId tagID)
        {
            EventEntry result = null;
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    result = IsCompetitorInEvent(tagID);
                });
            }
            else
            {
                result = IsCompetitorInEvent(tagID);
            }

            return result;
        }

        private EventEntry IsCompetitorInEvent(TagId tagID)
        {
            if (eventComboBox.SelectedIndex != -1)
            {
                Event currentEvent = DataManager.Instance.Events[eventComboBox.SelectedIndex];
                foreach (EventEntry eve in DataManager.Instance.EventEntries)
                {
                    if (eve.eventID == currentEvent.ID && (TagsEqual(tagID, eve.tagNumber) || TagsEqual(tagID, eve.tagNumber2)))
                    {
                        return eve;
                    }
                }
            }

            return null;
        }

        public bool TagsEqual(TagId t1, TagId t2)
        {
            if (t1.Value == null || t1.Value.Equals("0") || t1.Value.Equals(""))
                return false;

            if (t2.Value == null || t2.Value.Equals("0") || t2.Value.Equals(""))
                return false;

            if (t1.Value != null && !t1.Value.Equals("0") && !t1.Value.Equals("") && t1.Equals(t2))
                return true;

            return false;
        }

        public void RefreshPassingsGrid()
        {
            if (passingsDataGrid.DataSource == null) return;

            (passingsDataGrid.DataSource as BindingList<PassingsInfo>).ResetBindings();
        }

        public Race GetCurrentSession()
        {
            return currentRace;
        }


    }
}
