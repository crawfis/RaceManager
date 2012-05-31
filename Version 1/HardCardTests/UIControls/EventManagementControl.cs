using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EventProject;

namespace UIControls
{
    /// <summary>
    /// This control is used for creating and editing of events.
    /// </summary>
    public partial class EventManagementControl : UserControl
    {
        public DataGridView DataGrid
        {
            get { return eventsDataGridView; }
            set { eventsDataGridView = value; }
        }

        public Button CloseDialogButton
        {
            get { return closeDialogButton; }
            set { closeDialogButton = value; }
        }
        
        public EventManagementControl()
        {
            InitializeComponent();

            this.eventsDataGridView.DataSource = DataManager.Instance.Events;

            //make sure some data is loaded first! (or data is bound from external source)
            List<String> columnsToHide = new List<String>();
            columnsToHide.Add("ID");
            this.HideColumns(columnsToHide);

            activeClassesGridView.DataSource = DataManager.Instance.Classes;
            activeClassesGridView.Columns["classNumber"].Visible = false;

            this.validClassesGridView.DataSource = new BindingList<Class>();
            this.validClassesGridView.Columns["classNumber"].Visible = false;

            this.addEventDateButton.Click += new EventHandler(addEventDateButton_Click);
            this.editEventDateButton.Click += new EventHandler(editEventDateButton_Click);
            this.removeEventDateButton.Click += new EventHandler(removeEventDateButton_Click);

            this.activeToValidClassesButton.Click += new EventHandler(activeToValidClassesButton_Click);
            this.validToActiveClassesButton.Click += new EventHandler(validToActiveClassesButton_Click);

            this.addRaceButton.Click += new EventHandler(addRaceButton_Click);

            this.sessionTypeComboBox.SelectedIndex = 0;//set default

            this.eventRacesGridView.SelectionChanged += new EventHandler(eventRacesGridView_SelectionChanged);

            this.exportScheduleButton.Click += new EventHandler(exportScheduleButton_Click);
            this.importScheduleButton.Click += new EventHandler(importScheduleButton_Click);
        }

        private void importScheduleButton_Click(object sender, EventArgs e)
        {
            //make sure there is at least one event selected
            if (this.eventsDataGridView.SelectedRows.Count <= 0)
            {
                MessageBox.Show("Please select an event first!");
                return;
            }
            int selectionIndex = eventsDataGridView.SelectedRows[0].Index;
            Event selectedEvent = DataManager.Instance.Events[selectionIndex];
            
            OpenFileDialog fd = new OpenFileDialog();
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BindingList<Race> importedRaces = new BindingList<Race>();
                bool result = DataManager.Instance.ImportEventSchedule(fd.FileName, out importedRaces);
                if (!result)
                {
                    MessageBox.Show("Sorry, could not open the file.");
                    fd.Dispose();
                    return;
                }
                selectedEvent.races = importedRaces;
                eventsDataGridView_SelectionChanged(null, null);//refresh
            }
            fd.Dispose();
        }

        private void exportScheduleButton_Click(object sender, EventArgs e)
        {
            if (eventRacesGridView.Rows == null || eventRacesGridView.Rows.Count <= 0)
            {
                MessageBox.Show("Sorry, cannot export empty schedule!");
                return;
            }

            SaveFileDialog fd = new SaveFileDialog();
            fd.Title = "Export Event Schedule";
            fd.Filter = "CSV files (*.csv)|*.csv";
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                bool result = DataManager.Instance.ExportEventSchedule(fd.FileName, (eventRacesGridView.DataSource as BindingList<Race>));
                if (!result)
                    MessageBox.Show("Sorry, could not save the file.");
            }
            fd.Dispose();
        }

        //show classes in classes grid according to current session (race) selection
        private void eventRacesGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (eventRacesGridView.SelectedCells.Count > 0)
            {
                int rowIndex = eventRacesGridView.SelectedCells[0].RowIndex;
                Race race = (eventRacesGridView.DataSource as BindingList<Race>)[rowIndex];
                validClassesGridView.DataSource = race.validClasses;

                //put date for the race to the dates grid
                eventDatesGridView.DataSource = new BindingList<DateTime>();
                if(race.Dates != null)
                    eventDatesGridView.DataSource = race.Dates;
            }
            else
            {
                validClassesGridView.DataSource = new BindingList<Class>();
                eventDatesGridView.DataSource = new BindingList<DateTime>();
            }

            if (eventDatesGridView.Columns != null)
            {
                for (int i = 1; i < eventDatesGridView.Columns.Count; i++)
                    eventDatesGridView.Columns[i].Visible = false;
            }
        }

        //">>" button (removing class from a session)
        private void validToActiveClassesButton_Click(object sender, EventArgs e)
        {
            if (this.validClassesGridView.SelectedRows.Count > 0 && eventRacesGridView.SelectedCells.Count > 0)
            {
                int selectionIndex = validClassesGridView.SelectedRows[0].Index;
                //removing here means removing from the corresponding race (session), as they are bound
                (validClassesGridView.DataSource as BindingList<Class>).RemoveAt(selectionIndex);
            }
        }

        //"<<" button (adding class to a session)
        private void activeToValidClassesButton_Click(object sender, EventArgs e)
        {
            if (this.eventsDataGridView.SelectedRows.Count <= 0)
                return;
            //if (this.eventsDataGridView.SelectedRows.Count > 0)
            //{
            //    int selectionIndex = eventsDataGridView.SelectedRows[0].Index;
            //    Event selectedEvent = DataManager.Instance.Events[selectionIndex];

                if(activeClassesGridView.SelectedRows.Count > 0)//some active class is selected
                {
                    if (eventRacesGridView.SelectedCells.Count > 0)//some session (race) is selected
                    {
                        int classSelectionIndex = activeClassesGridView.SelectedRows[0].Index;
                        Class selectedClass = DataManager.Instance.Classes[classSelectionIndex];

                        int rowIndex = eventRacesGridView.SelectedCells[0].RowIndex;
                        Race race = (eventRacesGridView.DataSource as BindingList<Race>)[rowIndex];
                        if(!race.validClasses.Contains(selectedClass))
                            race.validClasses.Add(selectedClass);
                    }
                }
            //}    
        }

        public void ClearControls()
        {
            this.eventNameTextBox.Text = "";
            this.eventCityTextBox.Text = "";
            this.eventStateComboBox.SelectedIndex = -1;
        }

        public void HideColumns(List<String> columnsToHide)
        {
            foreach (String columnName in columnsToHide)
            {
                eventsDataGridView.Columns[columnName].Visible = false;
            }
        }

        public void ShowColumns(List<String> columnsToShow)
        {
            foreach (String columnName in columnsToShow)
            {
                eventsDataGridView.Columns[columnName].Visible = true;
            }
        }

        private void addEventButtonClicked(object sender, EventArgs e)
        {
            //add a new event with empty fields
            ClearControls();            
            Event newEvent = new Event();
            newEvent.name = "New Event";
            DataManager.Instance.Events.Add(newEvent);
            eventsDataGridView.DataSource = DataManager.Instance.Events;
            //set selection of the new event in the grid
            for (int i = 0; i < eventsDataGridView.Rows.Count; i++)
            {
                eventsDataGridView.Rows[i].Selected = false;
            }
            eventsDataGridView.Rows[eventsDataGridView.Rows.Count - 1].Selected = true;

            if (eventDatesGridView.Columns != null)
            {
                for (int i = 1; i < eventDatesGridView.Columns.Count; i++)
                    eventDatesGridView.Columns[i].Visible = false;
            }

            DataManager.Instance.SerializeData();
        }

        private void modifyEventButton_Click(object sender, EventArgs e)
        {
            if (this.eventsDataGridView.SelectedRows.Count > 0)
            {
                int selectionIndex = eventsDataGridView.SelectedRows[0].Index;
                Event selectedEvent = DataManager.Instance.Events[selectionIndex];
                selectedEvent.name = this.eventNameTextBox.Text;
                selectedEvent.city = this.eventCityTextBox.Text;
                selectedEvent.state = this.eventStateComboBox.Text;
                DataManager.Instance.Events[selectionIndex] = selectedEvent;//have to reset
            }
            else
            {
                MessageBox.Show("Please select a row in the table first");
                return;
            }

            if (eventDatesGridView.Columns != null)
            {
                for (int i = 1; i < eventDatesGridView.Columns.Count; i++)
                    eventDatesGridView.Columns[i].Visible = false;
            }

            DataManager.Instance.SerializeData();
        }

        private void removeEventButton_Click(object sender, EventArgs e)
        {
            if (this.eventsDataGridView.SelectedRows.Count > 0)
            {
                int selectionIndex = eventsDataGridView.SelectedRows[0].Index;
                if (MessageBox.Show("Are you sure you'd like to delete the selected event?", 
                    "Deletion Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Event ev = DataManager.Instance.Events[selectionIndex];

                    foreach (Race race in ev.races)
                    {
                        race.competitorRaceList.Clear();
                    //    foreach (CompetitorRace cr in race.competitorRaceList)
                    //    {
                    //        race.RemoveCompetitorRace(cr);
                    //    }
                    }
                    ev.races.Clear();

                    DataManager.Instance.Events.RemoveAt(selectionIndex);
                }
            }
            else
            {
                MessageBox.Show("Please select a row in the table first");
                return;
            }
            DataManager.Instance.SerializeData();
        }

        private void eventsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (eventsDataGridView.SelectedRows.Count > 0)
            {
                int selectionIndex = eventsDataGridView.SelectedRows[0].Index;
                Event ev = DataManager.Instance.Events[selectionIndex];
                this.eventNameTextBox.Text = ev.name;
                this.eventCityTextBox.Text = ev.city;
                this.eventStateComboBox.SelectedItem = ev.state;

                //populate dates grid according to what event has been selected
                //eventDatesGridView.DataSource = ev.dates;

                //have to do it due to present date nicely, showing only one field
                if (eventDatesGridView.Columns != null)
                {
                    for (int i = 1; i < eventDatesGridView.Columns.Count; i++)
                        eventDatesGridView.Columns[i].Visible = false;
                }
                
                validClassesGridView.DataSource = ev.classes;
                validClassesGridView.Columns["classNumber"].Visible = false;

                //
                eventRacesGridView.DataSource = ev.races;
                eventRacesGridView.Columns["ID"].Visible = false;
                eventRacesGridView.CellEndEdit += new DataGridViewCellEventHandler(eventRacesGridView_CellEndEdit);
                
                //change to standard buttons controls if cannot fix this!
                //!!!
                //eventRacesGridView.UserAddedRow += new DataGridViewRowEventHandler(eventRacesGridView_UserAddedRow);//does not work!!!
                //eventRacesGridView.CellEnter +=new DataGridViewCellEventHandler(eventRacesGridView_CellEnter);//does not work
                //eventRacesGridView.
                //just make that thing false:
                eventRacesGridView.AllowUserToAddRows = false;

                //in case there was a listener before, remove it first
                eventRacesGridView.UserDeletingRow -= eventRacesGridView_UserDeletingRow;
                eventRacesGridView.UserDeletedRow -= eventRacesGridView_UserDeletedRow;

                eventRacesGridView.UserDeletingRow += eventRacesGridView_UserDeletingRow;

                eventRacesGridView.UserDeletedRow += eventRacesGridView_UserDeletedRow;

                eventRacesGridView.SelectionChanged += new EventHandler(eventRacesGridView_SelectionChanged);
            }
        }

        private void addRaceButton_Click(object sender, EventArgs e)
        {
            //if ((eventDatesGridView.DataSource as BindingList<DateTime>) == null)
            //{
            //    MessageBox.Show("Please add event first and then add races to it!");
            //    return;
            //}
            if (this.eventsDataGridView.SelectedRows.Count <= 0)
            {
                MessageBox.Show("Please select (create if necessary) event and then add sessions to it!");
                return;
            }

            if (this.eventsDataGridView.SelectedRows.Count > 0)
            {
                Race newRace = new Race(addRaceTextField.Text);
                int selectedIndex = sessionTypeComboBox.SelectedIndex;
                newRace.type = sessionTypeComboBox.Items[selectedIndex] as String;
                (eventRacesGridView.DataSource as BindingList<Race>).Add(newRace);
                (eventRacesGridView.DataSource as BindingList<Race>).ResetBindings();
            }
            else if(this.eventsDataGridView.Rows.Count == 1)//one row in the grid, selected by default
            {
                Race newRace = new Race(addRaceTextField.Text);
                int selectedIndex = sessionTypeComboBox.SelectedIndex;
                newRace.type = sessionTypeComboBox.Items[selectedIndex] as String;
                (eventRacesGridView.DataSource as BindingList<Race>).Add(newRace);
                (eventRacesGridView.DataSource as BindingList<Race>).ResetBindings();
            }
            else
                MessageBox.Show("Please select a row in the table first.");
        }

        private void eventRacesGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            (eventRacesGridView.DataSource as BindingList<Race>).ResetBindings();
        }

        private void eventRacesGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you'd like to delete the selected session?",
                "Deletion Confirmation", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }
        
        //refresh after deletion
        private void eventRacesGridView_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            (eventRacesGridView.DataSource as BindingList<Race>).ResetBindings();
        }

        private void eventRacesGridView_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            (eventRacesGridView.DataSource as BindingList<Race>).ResetBindings();
        }

        //refresh after edit
        private void eventRacesGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            (eventRacesGridView.DataSource as BindingList<Race>).ResetBindings();
        }

        //TODO: associate one date (only one!) with a session
        #region Date Add, Edit, Remove
        private void removeEventDateButton_Click(object sender, EventArgs e)
        {
            if (this.eventDatesGridView.SelectedRows.Count > 0)
            {
                int selectionIndex = eventDatesGridView.SelectedRows[0].Index;
                if (MessageBox.Show("Are you sure you'd like to delete the selected date?",
                    "Deletion Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    (eventDatesGridView.DataSource as BindingList<DateTime>).RemoveAt(selectionIndex);
                }
            }
            else
                MessageBox.Show("Please select a row in the table first.");
        }

        private void editEventDateButton_Click(object sender, EventArgs e)
        {
            if (this.eventDatesGridView.SelectedRows.Count > 0)
            {
                int selectionIndex = eventDatesGridView.SelectedRows[0].Index;
                DateTime selectedDate = (eventDatesGridView.DataSource as BindingList<DateTime>)[selectionIndex];
                ShowEditDateDialog(selectedDate, true, selectionIndex);
            }
            else
                MessageBox.Show("Please select a row in the table first.");
        }

        private void addEventDateButton_Click(object sender, EventArgs e)
        {
            ShowEditDateDialog(new DateTime(), false, -1);
        }
        #endregion

        private void ShowEditDateDialog(DateTime selectedDate, bool useSelectedDate, int selectedIndex)
        {
            Race currentRace = null;
            if (eventRacesGridView.SelectedCells != null && eventRacesGridView.SelectedCells.Count > 0)//some session (race) is selected
            {
                int rowIndex = eventRacesGridView.SelectedCells[0].RowIndex;
                currentRace = (eventRacesGridView.DataSource as BindingList<Race>)[rowIndex];
            }
            else
            {
                MessageBox.Show("Please select (or create and select) a run first!");
                return;
            }

            Form popupForm = new Form();
            popupForm.Text = "Please select a date to add";
            popupForm.Width = 300;
            popupForm.Height = 150;
            DateTimePicker datePicker = new DateTimePicker();
            datePicker.Location = new Point(50, 20);
            try
            {
                if (useSelectedDate)
                    datePicker.Value = selectedDate;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.StackTrace);
                DataManager.Log(exc.StackTrace);
            }
            popupForm.Controls.Add(datePicker);
            Button okButton = new Button();
            okButton.Text = "OK";
            okButton.Location = new Point(popupForm.Width - 240, popupForm.Height - 60);
            Button cancelButton = new Button();
            cancelButton.Text = "Cancel";
            cancelButton.Location = new Point(popupForm.Width - 160, popupForm.Height - 60);
            popupForm.Controls.Add(okButton);
            popupForm.Controls.Add(cancelButton);

            okButton.Click += delegate(object s, EventArgs ea)
            {
                if ((eventDatesGridView.DataSource as BindingList<DateTime>) == null)
                {
                    MessageBox.Show("Please add event first and then add dates to it!");
                    popupForm.Dispose();
                    return;
                }

                //instead, add this date/modify existing date for the selected race (session)

                //if(!useSelectedDate)//add a new date
                //    (eventDatesGridView.DataSource as BindingList<DateTime>).Add(datePicker.Value);
                //else//modify existing one
                //    (eventDatesGridView.DataSource as BindingList<DateTime>)[selectedIndex] = datePicker.Value;

                if (currentRace == null)
                {
                    popupForm.Dispose();
                }
                if (!useSelectedDate)//add a new date
                {
                    //only one date should be allowed, thus clean the old data first
                    (eventDatesGridView.DataSource as BindingList<DateTime>).Clear();
                    (eventDatesGridView.DataSource as BindingList<DateTime>).Add(datePicker.Value);
                }
                else//modify existing one
                {
                    (eventDatesGridView.DataSource as BindingList<DateTime>)[selectedIndex] = datePicker.Value;
                }

                popupForm.Dispose();
            };

            popupForm.CancelButton = cancelButton;
            popupForm.ShowDialog(this);
        }

    }
}
