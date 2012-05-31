using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Hardcard.Scoring;
using UIControls;
using EventProject;

namespace UIControlsTesting
{
    public partial class UserControlsTestingForm : Form
    {
        public UserControlsTestingForm()
        {
            //this should go before any other initialization!
            //otherwise, in DataManager the orifinal "default" string will be used!
            //Config.Instance.UserKeyString = Properties.Settings.Default.userKeyString;
            //MessageBox.Show("user key string: " + Config.Instance.UserKeyString);

            InitializeComponent();

            this.Icon = Properties.Resources.Hardcardlogo2;//ico

            //give a third tab reference to second tab
            this.passingsManagementControl1.RaceInformationControl = this.raceInformationControl1;

            this.WindowState = FormWindowState.Maximized;
            this.FormClosed += new FormClosedEventHandler(UserControlsTestingForm_FormClosed);

            competitorDataInput.EnterButtonPressedEvent += 
                new UIControls.CompetitorDataInput.EnterButtonPressedHandler(competitorDataInput1_EnterButtonClickedEvent);

            competitorsDataGridControl.DataGridView.SelectionChanged += new EventHandler(CompetitorDataGridView_SelectionChanged);
            competitorsDataGridControl.DataGridView.VisibleChanged += new EventHandler(CompetitorDataGridView_VisibleChanged);
            competitorsDataGridControl.DataGridView.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(DataGridView_DataBindingComplete);

            addCompetitorButton.Click += new EventHandler(AddCompetitor_Click);
            editCompetitorButton.Click += new EventHandler(editCompetitorButton_Click);
            removeCompetitorButton.Click += new EventHandler(removeCompetitorButton_Click);
            clearCompetitorsUIButton.Click += new EventHandler(clearCompetitorsUIButton_Click);
            deleteAllCompetitorsButton.Click += new EventHandler(deleteAllCompetitorsButton_Click);
            deleteSelectedCompetitorsButton.Click += new EventHandler(deleteSelectedCompetitorsButton_Click);

            manageEventsButton.Click += new EventHandler(manageEventsButtonClick);
            
            competitorsDataGridControl.DataGridView.DataSource = DataManager.Instance.Competitors;
            
            eventsComboBox.DataSource = DataManager.Instance.Events;
            eventsComboBox.SelectedIndexChanged += new EventHandler(eventsComboBox_SelectedIndexChanged);
            eventsComboBox.VisibleChanged += new EventHandler(eventsComboBox_VisibleChanged);
            if (DataManager.Instance.Events.Count > 0)//init
            {
                selectedRaceComboBox.DataSource = DataManager.Instance.Events[0].races;
                if (DataManager.Instance.EventSelectedIndex != -1 && DataManager.Instance.Events.Count > DataManager.Instance.EventSelectedIndex)
                    selectedRaceComboBox.DataSource = DataManager.Instance.Events[DataManager.Instance.EventSelectedIndex].races;
            }
            if ((selectedRaceComboBox.DataSource as BindingList<Race>) != null && (selectedRaceComboBox.DataSource as BindingList<Race>).Count > 0)//init
            {
                raceCompetitorsGridView.DataSource = (selectedRaceComboBox.DataSource as BindingList<Race>)[0].competitorRaceList;
                raceCompetitorsGridView.Columns["competitor"].Visible = false;
                //raceCompetitorsGridView.Columns["currentPlace"].Visible = false;
                raceCompetitorsGridView.Columns["lastLap"].Visible = false;
                raceCompetitorsGridView.Columns["bestLap"].Visible = false;
                raceCompetitorsGridView.Columns["EventEntry"].Visible = false;
                raceCompetitorsGridView.Columns["raceParent"].Visible = false;
                if (raceCompetitorsGridView.Columns["currentPlace"] != null)
                    raceCompetitorsGridView.Columns["currentPlace"].DisplayIndex = 0;
            }

            //doesn't work if no data loaded first!
            List<String> columnsToHide = new List<String>();
            columnsToHide.Add("ID");
            columnsToHide.Add("Address");
            columnsToHide.Add("Phone");
            columnsToHide.Add("DOB");
            columnsToHide.Add("Age");
            columnsToHide.Add("Gender");
            columnsToHide.Add("Sponsors");
            columnsToHide.Add("BikeBrand");
            columnsToHide.Add("TagNumber");
            columnsToHide.Add("TagNumber2");
            competitorsDataGridControl.HideColumns(columnsToHide);

            selectedRaceComboBox.SelectedIndexChanged += new EventHandler(selectedRaceComboBox_SelectedIndexChanged);

            competitorEventEntriesGrid.SelectionChanged += new EventHandler(competitorEventEntriesGrid_SelectionChanged);

            addCompetitorsToRaceButton.Click += new EventHandler(addCompetitorsToRaceButton_Click);
            removeCompetitorsFromRaceButton.Click += new EventHandler(removeCompetitorsFromRaceButton_Click);

            //event entries 
            competitorEventEntriesGrid.DataSource = new BindingList<EventEntry>();
            addEntryButton.Click += new EventHandler(addEntryButton_Click);
            editEntryButton.Click += new EventHandler(editEntryButton_Click);
            removeEntryButton.Click += new EventHandler(removeEntryButton_Click);

            CreateMenus();

            Config.Instance.SleepTime = Properties.Settings.Default.sleepTime;
            Config.Instance.PortNumber = Properties.Settings.Default.portNumber;

            addCompetitorsToRunButton.Click += new EventHandler(addCompetitorsToRunButton_Click);
        }

        #region Menus and Menus Handlers
        private void CreateMenus()
        {
            MainMenu mainMenu = new MainMenu();
            this.Menu = mainMenu;
            
            MenuItem menuItemFile = new MenuItem("&File");
            MenuItem menuItemHelp = new MenuItem("&Help");
            //MenuItem menuItemLoadTagFile = new MenuItem("&Load Tag File");
            MenuItem menuItemExit = new MenuItem("&Exit");
            MenuItem menuItemAbout = new MenuItem("&About");
            //MenuItem menuItemValidTags = new MenuItem("&Show Valid Tags");
            MenuItem menuItemImportCompetitors = new MenuItem("Import Competitors");
            MenuItem menuItemExportCompetitors = new MenuItem("Export Competitors");
            MenuItem menuItemExportEventEntries = new MenuItem("Export Event Entries");
            MenuItem menuItemSetMinLapTime = new MenuItem("Set Minimum Lap Time");

            mainMenu.MenuItems.Add(menuItemFile);
            //menuItemFile.MenuItems.Add(menuItemLoadTagFile);
            //menuItemFile.MenuItems.Add(menuItemValidTags);
            menuItemFile.MenuItems.Add("-");
            menuItemFile.MenuItems.Add(menuItemImportCompetitors);
            menuItemFile.MenuItems.Add(menuItemExportCompetitors);
            menuItemFile.MenuItems.Add("-");
            menuItemFile.MenuItems.Add(menuItemExportEventEntries);
            menuItemFile.MenuItems.Add("-");
            menuItemFile.MenuItems.Add(menuItemSetMinLapTime);
            menuItemFile.MenuItems.Add("-");
            menuItemFile.MenuItems.Add(menuItemExit);

            mainMenu.MenuItems.Add(menuItemHelp);
            menuItemHelp.MenuItems.Add(menuItemAbout);

            //listeners
            //menuItemLoadTagFile.Click += new EventHandler(menuItemLoadTagFile_Click);
            menuItemExit.Click += new EventHandler(menuItemExit_Click);
            //menuItemValidTags.Click += new EventHandler(menuItemValidTags_Click);

            menuItemImportCompetitors.Click += new EventHandler(menuItemImportCompetitors_Click);
            menuItemExportCompetitors.Click += new EventHandler(menuItemExportCompetitors_Click);
            menuItemExportEventEntries.Click += new EventHandler(menuItemExportEventEntries_Click);

            menuItemSetMinLapTime.Click += new EventHandler(menuItemSetMinLapTime_Click);

            menuItemAbout.Click += new EventHandler(menuItemAbout_Click);
        }

        private void menuItemSetMinLapTime_Click(object sender, EventArgs e)
        {
            //show a dialog with a simple spinner to set minimum lap time
            Form setMinTimeDialog = new Form();
            setMinTimeDialog.Text = "Set Minimum Lap Time";
            setMinTimeDialog.Width = 220;
            setMinTimeDialog.Height = 160;

            NumericUpDown upDown = new NumericUpDown();
            upDown.Minimum = 10;
            upDown.Maximum = 999;
            upDown.Value = DataManager.Instance.MinimumLapTime;
            upDown.SetBounds(55, 50, 105, 30);
            setMinTimeDialog.Controls.Add(upDown);

            Label label = new Label();
            label.Text = "Seconds: ";
            label.SetBounds(2, 52, 53, 30);
            setMinTimeDialog.Controls.Add(label);
            
            Button okButton = new Button();
            okButton.Text = "OK";
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.SetBounds(55, 100, 50, 22);
            Button cancelButton = new Button();
            cancelButton.Text = "Cancel";
            cancelButton.SetBounds(110, 100, 50, 22);
            setMinTimeDialog.CancelButton = cancelButton;            

            setMinTimeDialog.Controls.Add(okButton);
            setMinTimeDialog.Controls.Add(cancelButton);

            setMinTimeDialog.ShowDialog();
            if (setMinTimeDialog.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                DataManager.Instance.MinimumLapTime = (int)upDown.Value;
            }
            setMinTimeDialog.Dispose();
        }


        private void menuItemExportEventEntries_Click(object sender, EventArgs e)
        {
            if (eventsComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select current event first");
                return;
            }

            SaveFileDialog fd = new SaveFileDialog();
            fd.Title = "Export Event Entries for current event";
            fd.Filter = "CSV files (*.csv)|*.csv";
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                bool result = DataManager.Instance.ExportEventEntries(fd.FileName);
                if (!result)
                    MessageBox.Show("Sorry, could not save the file.");
            }
            fd.Dispose();
        }

        private void menuItemExportCompetitors_Click(object sender, EventArgs e)
        {
            SaveFileDialog fd = new SaveFileDialog();
            fd.Title = "Export Competitors";
            fd.Filter = "CSV files (*.csv)|*.csv";
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                bool result = DataManager.Instance.ExportCompetitors(fd.FileName);
                if (!result)
                    MessageBox.Show("Sorry, could not save the file.");
            }
            fd.Dispose();
        }

        private void menuItemImportCompetitors_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show("Are you sure you would like to load the Competitors file?",
            //    "Please confirm load file.", MessageBoxButtons.YesNo) == DialogResult.No)
            //{
            //    return;
            //}

            bool append = false;
            if (MessageBox.Show("Would you like to append the new data to the existing competitors? If not, it would be replaced.",
                "Please select.", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                append = true;
            }

            OpenFileDialog fd = new OpenFileDialog();
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //bool result = DataManager.Instance.ImportCompetitors(fd.FileName);
                bool result = DataManager.Instance.ImportCompetitorsXLS(fd.FileName, append);
                if (!result)
                    MessageBox.Show("Sorry, could not import the file.");
            }
            fd.Dispose();


        }

        private void menuItemValidTags_Click(object sender, EventArgs e)
        {            
            Form showValidTagsDialog = new Form();
            showValidTagsDialog.AutoScroll = true;
            showValidTagsDialog.Text = "Valid Tags";
            showValidTagsDialog.Width = 420;
            showValidTagsDialog.Height = 540;
            Button closeButton = new Button();
            closeButton.Text = "Close";
            closeButton.SetBounds(showValidTagsDialog.Width - 110, showValidTagsDialog.Height - 65, 45, 22);
            showValidTagsDialog.Controls.Add(closeButton);
            showValidTagsDialog.CancelButton = closeButton;

            DataGridView validTagsView = new DataGridView();
            validTagsView.Width = showValidTagsDialog.Width - 30;
            validTagsView.Height = showValidTagsDialog.Height - 90;
            showValidTagsDialog.Controls.Add(validTagsView);

            if (DataManager.Instance.ValidTags != null)
            {
                DataTable t = new DataTable();
                t.Columns.Add("TagID");
                t.Columns.Add("First Used On");
                foreach (String key in DataManager.Instance.ValidTags.Keys)
                {
                    t.Rows.Add(new object[] { key, DataManager.Instance.ValidTags[key] });
                }

                validTagsView.DataSource = t;
            }

            showValidTagsDialog.ShowDialog();

            showValidTagsDialog.Dispose();
        }

        private void menuItemAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hardcard Inc., 2012\nversion 1.5");
        }

        private void menuItemLoadTagFile_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show("Are you sure you would like to load the new Tag File?",
            //    "Please confirm load file.", MessageBoxButtons.YesNo) == DialogResult.No)
            //{
            //    return;
            //}

            LoadTagsDialog ltd = new LoadTagsDialog();
            if (ltd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                bool result = DataManager.Instance.LoadTagFileEncrypted(ltd.EncryptedFileStr, ltd.KeyFileStr, ltd.CustomerID);
                if(!result)
                    MessageBox.Show("Sorry, could not load the file. Old tag entries still going to be used.");
            }
            ltd.Dispose();
/*
            //load new tag file
            OpenFileDialog fd = new OpenFileDialog();
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                bool result = DataManager.Instance.LoadTagFile(fd.FileName);
                if (!result)
                    MessageBox.Show("Sorry, could not load the file. Old tag entries still going to be used.");
            }
            fd.Dispose();
*/
        }

        private void menuItemExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you would like to exit the program?",
                "Please confirm exit.", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            //DataManager.Instance.SerializeData();
            this.Close();
            this.Dispose();
        }
        #endregion

        /// <summary>
        /// Deselect competitor list on load. Actual data binding happens 
        /// later than initialization of the control,
        /// thus deselecting in the constructor doesn't work and we have
        /// to listen to the event.
        /// </summary>
        private int dataBoundCount = 0;
        private void DataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //ugly hack! re-binding may happen not only on load, thus add this check;
            //couldn't find proper way of figuring out when we finally end up loading
            //everything and nothing is re-bound anymore
            if (dataBoundCount > 5) return;

            competitorsDataGridControl.DataGridView.ClearSelection();
            competitorDataInput.ClearControls();
            dataBoundCount++;
        }

        #region Event Entries Add, Remove, Update Handlers

        /// <summary>
        /// Whenever the user changes selection in user's list and/or events combo,
        /// update the event entries grid.
        /// </summary>
        private void RefreshEventEntriesDataGrid()
        {
            Competitor c = null;
            Event ev = null;

            List<Competitor> competitors = GetSelectedCompetitors();
            if (competitors.Count > 0)
            {
                c = competitors[0];
            }
            else
            {
                competitorEventEntriesGrid.DataSource = new BindingList<EventEntry>();
                return;
            }

            if (eventsComboBox.SelectedIndex != -1)
                ev = DataManager.Instance.Events[eventsComboBox.SelectedIndex];
            else
            {
                competitorEventEntriesGrid.DataSource = new BindingList<EventEntry>();
                return;
            }

            competitorEventEntriesGrid.DataSource = new BindingList<EventEntry>();
            //find event entries corresponding to c and ev
            foreach(EventEntry eventEntry in DataManager.Instance.EventEntries)
            {
                if (eventEntry.competitorID == c.ID && eventEntry.eventID == ev.ID)
                {
                    (competitorEventEntriesGrid.DataSource as BindingList<EventEntry>).Add(eventEntry);
                }
            }

            foreach (DataGridViewColumn column in competitorEventEntriesGrid.Columns)
            {
                column.Visible = false;
            }
            competitorEventEntriesGrid.Columns["bikeNumber"].Visible = true;
            competitorEventEntriesGrid.Columns["className"].Visible = true;
            competitorEventEntriesGrid.Columns["tagNumber"].Visible = true;
            competitorEventEntriesGrid.Columns["tagNumber2"].Visible = true;
            competitorEventEntriesGrid.Columns["bikeBrand"].Visible = true;

            (competitorEventEntriesGrid.DataSource as BindingList<EventEntry>).ResetBindings();

            classComboBox.SelectedIndex = -1;
            int selectedIndex = selectedRaceComboBox.SelectedIndex;
            if (selectedIndex != -1)
            {
                classComboBox.Items.Clear();
                foreach (Class cl in (selectedRaceComboBox.DataSource as BindingList<Race>)[selectedIndex].validClasses)
                {
                    classComboBox.Items.Add(cl.name);
                }
                if (classComboBox.Items.Count > 0)
                {
                    //classComboBox.SelectedIndex = 0;
                }
                else
                    classComboBox.Items.Clear();
            }
            else
                classComboBox.Items.Clear();
        }
        
        private void addEntryButton_Click(object sender, EventArgs e)
        {
            //save event entry for the given competitor for the given event
            //don't do if no event or competitor is selected
            Competitor c = null;
            Event ev = null;
            List<Competitor> competitors = GetSelectedCompetitors();
            if (competitors.Count > 0)
            {
                c = competitors[0];
            }
            else
            {
                MessageBox.Show("Please select only one competitor in competitors list!");
                return;
            }

            if (eventsComboBox.SelectedIndex != -1)
                ev = DataManager.Instance.Events[eventsComboBox.SelectedIndex];
            else
            {
                MessageBox.Show("Please select an event!");
                return;
            }

            try
            {
                EventEntry eventEntry = new EventEntry();
                eventEntry.competitorID = c.ID;
                eventEntry.eventID = ev.ID;
                //FIX WITH COMBO!
                //eventEntry.className = eventEntryClassTextBox.Text;
                if (classComboBox.SelectedIndex != -1)
                {
                    eventEntry.className = classComboBox.Items[classComboBox.SelectedIndex] as String;
                }
                else
                    eventEntry.className = "";
                eventEntry.bikeBrand = eventEntryBikeBrandTextBox.Text;
                try
                {
                    eventEntry.bikeNumber = eventEntryBikeNumTextBox.Text;// int.Parse(eventEntryBikeNumTextBox.Text);
                }
                catch
                {
                    eventEntry.bikeNumber = "0";
                }
                eventEntry.tagNumber = new TagId(eventEntryBikeTagTextBox.Text);
                eventEntry.tagNumber2 = new TagId(eventEntryBikeTag2TextBox.Text);
                eventEntry.competitor = c;//will reset info that is ambiguous

                DataManager.Instance.EventEntries.Add(eventEntry);
                (competitorEventEntriesGrid.DataSource as BindingList<EventEntry>).Add(eventEntry);
                (competitorEventEntriesGrid.DataSource as BindingList<EventEntry>).ResetBindings();

                DeselectEventEntriesUI();

                //have to check whether this is correct behavior
                if (c == null) return;
                //fill event entries UI with some default values from competitor, 
                //even if nothing is selected in the event entries grid
                eventEntryBikeBrandTextBox.Text = c.BikeBrand;
                eventEntryBikeNumTextBox.Text = "" + c.BikeNumber;
                eventEntryBikeTagTextBox.Text = "" + c.TagNumber;
                eventEntryBikeTag2TextBox.Text = "" + c.TagNumber2;
            }
            catch (Exception exc)
            {
                MessageBox.Show("There was a problem adding your event entry.");
            }
        }

        private void editEntryButton_Click(object sender, EventArgs e)
        {
            if (competitorEventEntriesGrid.SelectedRows.Count <= 0)
                return;
            
            int selectionIndex = competitorEventEntriesGrid.SelectedRows[0].Index;
            EventEntry eventEntry = (competitorEventEntriesGrid.DataSource as BindingList<EventEntry>)[selectionIndex];

            /*
            if (MessageBox.Show("Are you sure you would like to update the entry?", 
                "Please confirm edit.", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }
            */

            try
            {
                String newBikeNumber = eventEntryBikeNumTextBox.Text;//int.Parse(eventEntryBikeNumTextBox.Text);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Sorry, there was a problem with the bike number entered.");
                return;
            }

            //FIX WITH COMBO!
            //eventEntry.className = eventEntryClassTextBox.Text;
            if (classComboBox.SelectedIndex != -1)
            {
                eventEntry.className = classComboBox.Items[classComboBox.SelectedIndex] as String;
            }
            else
                eventEntry.className = "";

            eventEntry.bikeBrand = eventEntryBikeBrandTextBox.Text;
            try
            {
                eventEntry.bikeNumber = eventEntryBikeNumTextBox.Text;// int.Parse(eventEntryBikeNumTextBox.Text);
            }
            catch
            {
                eventEntry.bikeNumber = "0";
            }
            eventEntry.tagNumber = new TagId(eventEntryBikeTagTextBox.Text);
            eventEntry.tagNumber2 = new TagId(eventEntryBikeTag2TextBox.Text);
            (competitorEventEntriesGrid.DataSource as BindingList<EventEntry>).ResetBindings();
            DataManager.Instance.Competitors.ResetBindings();
        }

        private void removeEntryButton_Click(object sender, EventArgs e)
        {
            if (competitorEventEntriesGrid.SelectedRows.Count > 0)
            {
                int selectionIndex = competitorEventEntriesGrid.SelectedRows[0].Index;
                EventEntry eventEntry = (competitorEventEntriesGrid.DataSource as BindingList<EventEntry>)[selectionIndex];

                if (MessageBox.Show("Are you sure you would like to remove the selected entry?", 
                    "Please confirm edit", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }

                DataManager.Instance.EventEntries.Remove(eventEntry);

                (competitorEventEntriesGrid.DataSource as BindingList<EventEntry>).RemoveAt(selectionIndex);
                (competitorEventEntriesGrid.DataSource as BindingList<EventEntry>).ResetBindings();
            }
        }

        private void competitorEventEntriesGrid_SelectionChanged(object sender, EventArgs e)
        {
            int selectedIndex = -1;
            if (competitorEventEntriesGrid.SelectedRows.Count > 0)
                selectedIndex = competitorEventEntriesGrid.SelectedRows[0].Index;
            else
                return;

            //DeselectEventEntriesUI();
            
            EventEntry eve = (competitorEventEntriesGrid.DataSource as BindingList<EventEntry>)[selectedIndex];

            //FIX WITH COMBO!
            //eventEntryClassTextBox.Text = eve.className;
            classComboBox.SelectedIndex = -1;//deselect by default
            foreach (String className in classComboBox.Items)
            {
                if (className.Equals(eve.className))
                {
                    classComboBox.SelectedItem = className;
                    break;
                }
            }

            eventEntryBikeBrandTextBox.Text = eve.bikeBrand;
            eventEntryBikeNumTextBox.Text = "" + eve.bikeNumber;
            eventEntryBikeTagTextBox.Text = "" + eve.tagNumber;
            eventEntryBikeTag2TextBox.Text = "" + eve.tagNumber2;
            eventEntryBikeBrandTextBox.Text = "" + eve.bikeBrand;
        }
        #endregion

        private void DeselectEventEntriesUI()
        {
            competitorEventEntriesGrid.ClearSelection();
            eventEntryBikeNumTextBox.Text = "";
            eventEntryBikeTagTextBox.Text = "";
            eventEntryBikeTag2TextBox.Text = "";
            eventEntryBikeBrandTextBox.Text = "";
        }

        #region Races Add, Remove Handlers
        private void addCompetitorsToRaceButton_Click(object sender, EventArgs e)
        {
            if (GetSelectedCompetitors().Count == 0)
            {
                MessageBox.Show("Please select a competitor in a list");
                return;
            }
            //if ((raceCompetitorsGridView.DataSource as BindingList<CompetitorRace>) == null)
            if ((raceCompetitorsGridView.DataSource as SortableBindingList<CompetitorRace>) == null)
            {
                MessageBox.Show("Please add a race to an event first!");
                return;
            }

            Race selectedRace = null;
            int index = selectedRaceComboBox.SelectedIndex;
            if (index != -1)
            {
                selectedRace = (selectedRaceComboBox.DataSource as BindingList<Race>)[index];
            }
            if (selectedRace == null) return;

            foreach (EventEntry eventEntry in GetSelectedEventEntries())
            {
                CompetitorRace cr = new CompetitorRace(eventEntry.competitorID);
                cr.EventEntry = eventEntry;
                cr.tagID = eventEntry.tagNumber;
                cr.tagID2 = eventEntry.tagNumber2;
                cr.bikeNumber = eventEntry.bikeNumber;
                cr.className = eventEntry.className;
                cr.raceParent = selectedRace;
                //(raceCompetitorsGridView.DataSource as BindingList<CompetitorRace>).Add(cr);
                //instead, add cr to a corresponding race
                bool entryExists = false;
                foreach (CompetitorRace existingCR in selectedRace.competitorRaceList)
                {
                    if (existingCR.isNull) continue;

                    //ugly! maybe add a method like "isValid()" to TagId class, that checks for null, 0, and empty string?
                    if ((existingCR.tagID.Value != null && !existingCR.tagID.Value.Equals("0") && !existingCR.tagID.Value.Equals("") && existingCR.tagID.Equals(cr.tagID)) ||
                        (existingCR.tagID.Value != null && !existingCR.tagID.Value.Equals("0") && !existingCR.tagID.Value.Equals("") && existingCR.tagID.Equals(cr.tagID2)) ||
                        (existingCR.tagID2.Value != null && !existingCR.tagID2.Value.Equals("0") && !existingCR.tagID2.Value.Equals("") && existingCR.tagID2.Equals(cr.tagID)) ||
                        (existingCR.tagID2.Value != null && !existingCR.tagID2.Value.Equals("0") && !existingCR.tagID2.Value.Equals("") && existingCR.tagID2.Equals(cr.tagID2)))
                    {
                        entryExists = true;
                        break;
                    }
                }

                //show dialog to confirm
                if (entryExists)
                {
                    if (MessageBox.Show("Entry with the same tag id(s) already in the list. Are you sure still want to add it?",
                        "Confirmation Dialog", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        continue;//don't add
                    }
                }
                
                //check if this competitor race already exists (check name, tag, etc)
                selectedRace.AddCompetitorRace(cr);
                
                raceCompetitorsGridView.Columns["competitor"].Visible = false;
            }
        }

        private void removeCompetitorsFromRaceButton_Click(object sender, EventArgs e)
        {
            List<CompetitorRace> races = GetSelectedRaces();

            if (races.Count <= 0)
            {
                MessageBox.Show("Please select a race in a list");
                return;
            }
            foreach (CompetitorRace cr in races)
            {
                if (cr.isNull) 
                    continue;
                
                //(raceCompetitorsGridView.DataSource as BindingList<CompetitorRace>).Remove(cr);
                //instead, find a corresponding race, remove from that!
                int index = selectedRaceComboBox.SelectedIndex;
                if (index != -1)
                {
                    Race selectedRace = (selectedRaceComboBox.DataSource as BindingList<Race>)[index];
                    selectedRace.RemoveCompetitorRace(cr);
                }
            }
        }
        #endregion

        /// <summary>
        /// This handler creates event entries for all selected competitors and adds
        /// them to the selected race (session, run).
        /// </summary>
        private void addCompetitorsToRunButton_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show("Are you sure you would like to add all the selected competitors to " + 
            //    "the selected run? This will create event entries for each of the competitors!",
            //    "Please confirm", MessageBoxButtons.YesNo) == DialogResult.No)
            //{
            //    return;
            //}
            
            if (GetSelectedCompetitors().Count == 0)
            {
                MessageBox.Show("Please select at least one competitor in a list!");
                return;
            }
            //if ((raceCompetitorsGridView.DataSource as BindingList<CompetitorRace>) == null)
            if ((raceCompetitorsGridView.DataSource as SortableBindingList<CompetitorRace>) == null)
            {
                MessageBox.Show("Please add a race to an event first!");
                return;
            }

            Race selectedRace = null;
            int index = selectedRaceComboBox.SelectedIndex;
            if (index != -1)
            {
                selectedRace = (selectedRaceComboBox.DataSource as BindingList<Race>)[index];
            }
            if (selectedRace == null) return;

            //for each competitor, create a new event entry; then add those event entries to the race
            Event ev = null;
            if (eventsComboBox.SelectedIndex != -1)
                ev = DataManager.Instance.Events[eventsComboBox.SelectedIndex];
            else
            {
                MessageBox.Show("Please select an event!");
                return;
            }
            try
            {
                foreach (Competitor c in GetSelectedCompetitors())
                {
                    String eventEntryClass = "";
                    if (classComboBox.SelectedIndex != -1 && classComboBox.Items != null && classComboBox.Items.Count > 0)
                    {
                        eventEntryClass = classComboBox.Items[classComboBox.SelectedIndex] as String;
                    }

                    EventEntry eventEntry = new EventEntry();
                    eventEntry.competitor = c;
                    eventEntry.competitorID = c.ID;
                    eventEntry.eventID = ev.ID;
                    eventEntry.tagNumber = c.TagNumber;
                    eventEntry.tagNumber2 = c.TagNumber2;
                    eventEntry.bikeNumber = c.BikeNumber;
                    eventEntry.bikeBrand = c.BikeBrand;
                    eventEntry.className = eventEntryClass;
                    DataManager.Instance.EventEntries.Add(eventEntry);

                    CompetitorRace cr = new CompetitorRace(eventEntry.competitorID);
                    cr.competitor = c;
                    cr.EventEntry = eventEntry;
                    cr.tagID = eventEntry.tagNumber;
                    cr.tagID2 = eventEntry.tagNumber2;
                    cr.bikeNumber = eventEntry.bikeNumber;
                    cr.className = eventEntry.className;
                    cr.raceParent = selectedRace;

                    selectedRace.AddCompetitorRace(cr);
                    raceCompetitorsGridView.Columns["competitor"].Visible = false;
                }

                //deselect competitors after adding
                competitorsDataGridControl.DataGridView.ClearSelection();
                competitorDataInput.ClearControls();//sets focus to last name text box as well
                //DeselectEventEntriesUI();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.StackTrace);
                DataManager.Log(exc.StackTrace);
                MessageBox.Show("Sorry, there was a problem when adding competitors.");
            }
        }

        private void selectedRaceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = selectedRaceComboBox.SelectedIndex;
            if (index != -1)
            {
                raceCompetitorsGridView.DataSource = (selectedRaceComboBox.DataSource as BindingList<Race>)[index].competitorRaceList;
                DataManager.Instance.SessionSelectedIndex = index;

                if(raceCompetitorsGridView.Columns["currentPlace"] != null)
                    raceCompetitorsGridView.Columns["currentPlace"].DisplayIndex = 0;
            }

            //FIX: populate CLASS combo with correct classes
            int selectedIndex = selectedRaceComboBox.SelectedIndex;
            if (selectedIndex != -1)
            {
                classComboBox.Items.Clear();
                foreach(Class cl in (selectedRaceComboBox.DataSource as BindingList<Race>)[selectedIndex].validClasses)
                {
                    classComboBox.Items.Add(cl.name);
                }
                if (classComboBox.Items.Count > 0)
                {
                    //classComboBox.SelectedIndex = 0;
                }
                else
                    classComboBox.Items.Clear();
            }
            else
                classComboBox.Items.Clear();
        }

        private void eventsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (eventsComboBox.SelectedIndex != -1 && DataManager.Instance.Events.Count > eventsComboBox.SelectedIndex)
            {
                selectedRaceComboBox.DataSource = DataManager.Instance.Events[eventsComboBox.SelectedIndex].races;
                DataManager.Instance.EventSelectedIndex = eventsComboBox.SelectedIndex;
            }

            RefreshEventEntriesDataGrid();
        }

        private void eventsComboBox_VisibleChanged(object sender, EventArgs e)
        {
            if (DataManager.Instance.EventSelectedIndex != -1 && DataManager.Instance.Events.Count > 0)
            {
                eventsComboBox.SelectedIndex = DataManager.Instance.EventSelectedIndex;
                if (DataManager.Instance.Events.Count > DataManager.Instance.EventSelectedIndex)
                {
                    selectedRaceComboBox.DataSource = DataManager.Instance.Events[DataManager.Instance.EventSelectedIndex].races;

                    if (DataManager.Instance.SessionSelectedIndex != -1 &&
                        (selectedRaceComboBox.DataSource as BindingList<Race>).Count > DataManager.Instance.SessionSelectedIndex)
                    {
                        selectedRaceComboBox.SelectedIndex = DataManager.Instance.SessionSelectedIndex;
                    }
                }
            }

            RefreshEventEntriesDataGrid();
        }

        private void CompetitorDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            List<Competitor> competitors = GetSelectedCompetitors();
            Competitor c = null;
            if (competitors.Count > 0)
            {
                competitorDataInput.SetData(competitors[0]);
                c = competitors[0];
            }

            RefreshEventEntriesDataGrid();
            DeselectEventEntriesUI();

            if (c == null) return;
            //fill event entries UI with some default values from competitor, 
            //even if nothing is selected in the event entries grid
            eventEntryBikeBrandTextBox.Text = c.BikeBrand;
            eventEntryBikeNumTextBox.Text = "" + c.BikeNumber;
            eventEntryBikeTagTextBox.Text = "" + c.TagNumber;
            eventEntryBikeTag2TextBox.Text = "" + c.TagNumber2;
        }

        private void CompetitorDataGridView_VisibleChanged(object sender, EventArgs e)
        {
            if (!competitorsDataGridControl.DataGridView.Visible)
                return;

            List<Competitor> competitors = GetSelectedCompetitors();
            if (competitors.Count > 0)
                competitorDataInput.SetData(competitors[0]);
            RefreshEventEntriesDataGrid();
        }

        private EventManagementControl eventManagementControl;
        private void manageEventsButtonClick(object sender, EventArgs e)
        {
            if (eventManagementControl == null || eventManagementControl.IsDisposed)
            {
                eventManagementControl = new EventManagementControl();
            }

            Form dialogForm = new Form();
            dialogForm.Text = "Manage Events";
            dialogForm.Controls.Add(eventManagementControl);
            dialogForm.CancelButton = eventManagementControl.CloseDialogButton;
            dialogForm.Width = eventManagementControl.Width + 20;//add some offset
            dialogForm.Height = eventManagementControl.Height + 30;//add some offset

            dialogForm.ShowDialog(this);

            dialogForm.Dispose();

            DataManager.Instance.SerializeData();
        }

        #region Competitor Add, Edit, Remove Handlers
        void competitorDataInput1_EnterButtonClickedEvent()
        {
            AddCompetitor_Click(null, null);
        }

        private void AddCompetitor_Click(object sender, EventArgs e)
        {
            //get data from the data input table and put into DataGridControl
            int id;
            String lastName;
            String firstName;
            Address address;
            PhoneNumber phone;
            DateTime dob;
            int age;
            bool gender;
            String sponsors;
            String bikeBrand;
            String bikeNumber;
            TagId tagNumber;
            TagId tagNumber2;
            bool propagateChanges;

            String errorMessage;

            bool dataResult = competitorDataInput.GetData(out id, out lastName, out firstName, out address, out phone, out dob, 
                out age, out gender, out sponsors, out bikeBrand, out bikeNumber, out tagNumber, out tagNumber2, out errorMessage, out propagateChanges);
            if (!dataResult)
            {
                MessageBox.Show(errorMessage);
                return;
            }

            //check whether such a user exists already (identified by Last Name/First Name)
            bool competitorExists = false;
            foreach(Competitor c in DataManager.Instance.Competitors)
            {
                if (c.LastName == lastName && c.FirstName == firstName)
                {
                    competitorExists = true;
                    break;
                }
            }
            if (competitorExists)
            {
                if (MessageBox.Show("Would you really like to add a competitor with the first name and last name that is already in the list?",
                    "Confirmation Dialog", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            }

            Competitor newCompetitor = new Competitor(id, lastName, firstName, address, phone, dob, age, gender, sponsors, bikeBrand, bikeNumber, tagNumber, tagNumber2);
            newCompetitor.propagateChanges = propagateChanges;
            DataManager.Instance.Competitors.Add(newCompetitor);

            competitorDataInput.ClearControls();
            DeselectEventEntriesUI();

            DataManager.Instance.SerializeData();
        }

        private void editCompetitorButton_Click(object sender, EventArgs e)
        {
            List<Competitor> competitors = GetSelectedCompetitors();
            
            if (competitors.Count == 0)
            {
                MessageBox.Show("Please select a competitor in a list");
                return;
            }
            Competitor c = competitors[0];

            int id;
            String lastName;
            String firstName;
            Address address;
            PhoneNumber phone;
            DateTime dob;
            int age;
            bool gender;
            String sponsors;
            String bikeBrand;
            String bikeNumber;
            TagId tagNumber;
            TagId tagNumber2;
            String errorMessage;
            bool propagateChanges;

            bool dataResult = competitorDataInput.GetData(out id, out lastName, out firstName, out address, out phone, out dob,
                out age, out gender, out sponsors, out bikeBrand, out bikeNumber, out tagNumber, out tagNumber2, out errorMessage, out propagateChanges);
            if (!dataResult)
            {
                MessageBox.Show(errorMessage);
                return;
            }
            //check whether such a user exists already (identified by Last Name/First Name)
            bool competitorExists = false;
            foreach (Competitor cm in DataManager.Instance.Competitors)
            {
                if (cm.LastName == lastName && cm.FirstName == firstName && cm.ID != c.ID)
                {
                    competitorExists = true;
                    break;
                }
            }
            if (competitorExists)
            {
                if (MessageBox.Show("Would you really like to edit a competitor so that the first name and last name are the same as an entry already in the list?",
                    "Confirmation Dialog", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            }

            /*
            if (MessageBox.Show("Would you really like to edit information for the selected competitor?",
                "Confirmation Dialog", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                //reset newly entered information in the text fields, etc.? Probably not.
                return;
            }
            */

            //update the competitor with the new data
            //don't reset the id!
            c.propagateChanges = propagateChanges;
            c.LastName = lastName;
            c.FirstName = firstName;
            c.Address = address;
            c.Phone = phone;
            c.DOB = dob;
            c.Age = age;
            c.Gender = gender;
            c.Sponsors = sponsors;
            c.BikeBrand = bikeBrand;
            c.BikeNumber = bikeNumber;
            c.TagNumber = tagNumber;
            c.TagNumber2 = tagNumber2;

            DataManager.Instance.Competitors.ResetBindings();//[selectionIndex] = c;//have to reset
            DataManager.Instance.SerializeData();
        }

        private void removeCompetitorButton_Click(object sender, EventArgs e)
        {
            List<Competitor> competitors = GetSelectedCompetitors();

            if (competitors.Count == 0)
            {
                MessageBox.Show("Please select a competitor in a list");
                return;
            }
            Competitor c = competitors[0];

            if (MessageBox.Show("Would you really like to remove the selected competitor " + c.FirstName + " " + c.LastName + "?",
                "Confirmation Dialog", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            DataManager.Instance.Competitors.Remove(c);

            competitorDataInput.ClearControls();
            DeselectEventEntriesUI();
            DataManager.Instance.SerializeData();
        }

        private void deleteSelectedCompetitorsButton_Click(object sender, EventArgs e)
        {
            List<Competitor> competitors = GetSelectedCompetitors();

            if (competitors.Count == 0)
            {
                MessageBox.Show("Please select competitor(s) in a list.");
                return;
            }

            if (MessageBox.Show("Would you really like to remove the selected competitors? You won't be able to undo this!",
                "Confirmation Dialog", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            foreach (Competitor c in competitors)
            {
                DataManager.Instance.Competitors.Remove(c);
            }

            competitorDataInput.ClearControls();
            DeselectEventEntriesUI();
            DataManager.Instance.SerializeData();
        }

        private void deleteAllCompetitorsButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Would you really like to remove ALL the competitors? You won't be able to undo this!",
                "Confirmation Dialog", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            DataManager.Instance.Competitors.Clear();
            competitorDataInput.ClearControls();
            DeselectEventEntriesUI();
            DataManager.Instance.SerializeData();
        }

        private void clearCompetitorsUIButton_Click(object sender, EventArgs e)
        {
            competitorsDataGridControl.DataGridView.ClearSelection();
            competitorDataInput.ClearControls();
            DeselectEventEntriesUI();
        }
        #endregion

        #region Auxiliary Methods
        /// <summary>
        /// Returns a list of competitors selected in the competitors view.
        /// If no one is selected, an empty list is returned.
        /// </summary>
        /// <returns></returns>
        private List<Competitor> GetSelectedCompetitors()
        {
            int selectedRowsCount = competitorsDataGridControl.DataGridView.SelectedRows.Count;
            if (selectedRowsCount == 0)
                return new List<Competitor>();

            List<Competitor> selectedCompetitors = new List<Competitor>();
            List<int> selectedIndexes = new List<int>();
            for (int i = 0; i < selectedRowsCount; i++)
            {
                selectedIndexes.Add(competitorsDataGridControl.DataGridView.SelectedRows[i].Index);
            }
            foreach (int index in selectedIndexes)
            {
                Competitor c = DataManager.Instance.Competitors[index];
                selectedCompetitors.Add(c);
            }

            return selectedCompetitors;
        }

        /// <summary>
        /// For the currently selected event, get all selected races from the races grid view.
        /// </summary>
        /// <returns></returns>
        private List<CompetitorRace> GetSelectedRaces()
        {
            if (raceCompetitorsGridView.SelectedRows.Count <= 0)
                return new List<CompetitorRace>();

            List<CompetitorRace> races = new List<CompetitorRace>();

            List<int> selectedIndices = new List<int>();
            int selectedRowsCount = raceCompetitorsGridView.SelectedRows.Count;
            for (int i = 0; i < selectedRowsCount; i++)
            {
                selectedIndices.Add(raceCompetitorsGridView.SelectedRows[i].Index);
            }
            foreach (int index in selectedIndices)
            {
                //races.Add((raceCompetitorsGridView.DataSource as BindingList<CompetitorRace>)[index]);
                races.Add((raceCompetitorsGridView.DataSource as SortableBindingList<CompetitorRace>)[index]);
            }

            return races;
        }

        /// <summary>
        /// Return currently selected event entries.
        /// </summary>
        /// <returns></returns>
        private List<EventEntry> GetSelectedEventEntries()
        {
            int selectedRowsCount = competitorEventEntriesGrid.SelectedRows.Count;
            if (selectedRowsCount == 0)
                return new List<EventEntry>();

            List<EventEntry> entries = new List<EventEntry>();
            List<int> selectedIndexes = new List<int>();
            for (int i = 0; i < selectedRowsCount; i++)
            {
                selectedIndexes.Add(competitorEventEntriesGrid.SelectedRows[i].Index);
            }
            foreach (int index in selectedIndexes)
            {
                EventEntry eventEntry = (competitorEventEntriesGrid.DataSource as BindingList<EventEntry>)[index];
                entries.Add(eventEntry);
            }
            return entries;
        }

        private void UserControlsTestingForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            DataManager.Instance.SerializeData();
            this.Dispose();
        }

        #endregion

        private void UserControlsTestingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.raceInformationControl1.Close();
        }
    }
}
