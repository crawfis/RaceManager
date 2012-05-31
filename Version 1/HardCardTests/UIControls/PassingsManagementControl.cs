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

namespace UIControls
{
    public partial class PassingsManagementControl : UserControl
    {
        private RaceInformationControl raceInformationControl;
        public RaceInformationControl RaceInformationControl
        {
            get { return raceInformationControl; }
            set 
            { 
                raceInformationControl = value;
            }
        }
        
        public PassingsManagementControl()
        {
            InitializeComponent();

            //make sure whenever someone opens this tab (control is in the tab), 
            //we show same competitor races as in the race management control
            competitorRaceDataGrid.VisibleChanged += new EventHandler(competitorRaceDataGrid_VisibleChanged);
            competitorRaceDataGrid.SelectionChanged += new EventHandler(competitorRaceDataGrid_SelectionChanged);

            addPassingButton.Click += new EventHandler(addPassingButton_Click);
            removePassingButton.Click += new EventHandler(removePassingButton_Click);
            undeletePassingButton.Click += new EventHandler(undeletePassingButton_Click);
        }

        private void addPassingButton_Click(object sender, EventArgs e)
        {
            if(raceInformationControl.GetCurrentRace() == null)
            {
                MessageBox.Show("Please select a race in the Race Info tab first!");
                return;
            }

            long existingTime = 1;
            int index = -1;
            if (passingsDataGrid.SelectedRows.Count != 0)
            {
                index = passingsDataGrid.SelectedRows[0].Index;
                PassingsInfo pi = (passingsDataGrid.DataSource as BindingList<PassingsInfo>)[index];
                existingTime = pi.Time + 1;//- 1;
            }

            CompetitorRace selectedCR = null;
            if (competitorRaceDataGrid.SelectedRows != null && competitorRaceDataGrid.SelectedRows.Count > 0)
            {
                int crIndex = competitorRaceDataGrid.SelectedRows[0].Index;
                
                selectedCR = (competitorRaceDataGrid.DataSource as SortableBindingList<CompetitorRace>)[crIndex];
            }

            Form addPassingDialog = new Form();
            addPassingDialog.Text = "Add a new passing";
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
                raceInformationControl.UpdatePassingsGrid(newTagArgs, index);
                raceInformationControl.UpdateRaceStandingsGrid(newTagArgs);

                SelectCompetitorRace(selectedCR);
            }

            addPassingDialog.Dispose();
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

            CompetitorRace selectedCR = null;
            if (competitorRaceDataGrid.SelectedRows != null && competitorRaceDataGrid.SelectedRows.Count > 0)
            {
                int crIndex = competitorRaceDataGrid.SelectedRows[0].Index;

                selectedCR = (competitorRaceDataGrid.DataSource as SortableBindingList<CompetitorRace>)[crIndex];
            }

            int index = passingsDataGrid.SelectedRows[0].Index;
            PassingsInfo pi = (passingsDataGrid.DataSource as BindingList<PassingsInfo>)[index];
            pi.LapTime = "-1";
            pi.Deleted = "DELETED";

            raceInformationControl.UpdateRaceStandingsGrid(null);

            SelectCompetitorRace(selectedCR);
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

            CompetitorRace selectedCR = null;
            if (competitorRaceDataGrid.SelectedRows != null && competitorRaceDataGrid.SelectedRows.Count > 0)
            {
                int crIndex = competitorRaceDataGrid.SelectedRows[0].Index;

                selectedCR = (competitorRaceDataGrid.DataSource as SortableBindingList<CompetitorRace>)[crIndex];
            }

            int index = passingsDataGrid.SelectedRows[0].Index;
            PassingsInfo pi = (passingsDataGrid.DataSource as BindingList<PassingsInfo>)[index];
            pi.Deleted = "";

            raceInformationControl.UpdateRaceStandingsGrid(null);

            SelectCompetitorRace(selectedCR);
        }

        //after adding/deleting/undeleting tags for a given CR,
        //re-select it in the competitor race grid
        private void SelectCompetitorRace(CompetitorRace cr)
        {
            if (cr == null) return;
            int selectedIndex = -1;
            int currentIndex = -1;
            foreach (CompetitorRace cmpR in (competitorRaceDataGrid.DataSource as SortableBindingList<CompetitorRace>))
            {
                currentIndex++;
                if (cmpR.Equals(cr))
                {
                    selectedIndex = currentIndex;
                    break;
                }
            }
            if (selectedIndex == -1)
                return;

            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    competitorRaceDataGrid.Rows[selectedIndex].Selected = true;
                });
            }
            else
            {
                competitorRaceDataGrid.Rows[selectedIndex].Selected = true;
            }
        }

        private void competitorRaceDataGrid_VisibleChanged(object sender, EventArgs e)
        {
            if (DataManager.Instance.EventSelectedIndex != -1 && DataManager.Instance.SessionSelectedIndex != -1 &&
                DataManager.Instance.Events != null && DataManager.Instance.Events.Count > 0)
            {
                Event currentEvent = DataManager.Instance.Events[DataManager.Instance.EventSelectedIndex];
                Race currentRace = currentEvent.races[DataManager.Instance.SessionSelectedIndex];

                currentRaceLabel.Text = currentRace == null ? "" : currentRace.name;
                if (raceInformationControl != null && currentRace == raceInformationControl.GetCurrentRace())
                {
                    addPassingButton.Enabled = true;
                }
                else
                    addPassingButton.Enabled = false;

                competitorRaceDataGrid.DataSource = currentRace.competitorRaceList;

                if (competitorRaceDataGrid.Columns["competitor"] != null)
                    competitorRaceDataGrid.Columns["competitor"].Visible = false;
                if (competitorRaceDataGrid.Columns["raceParent"] != null)
                    competitorRaceDataGrid.Columns["raceParent"].Visible = false;
                if (competitorRaceDataGrid.Columns["EventEntry"] != null)
                    competitorRaceDataGrid.Columns["EventEntry"].Visible = false;
                if (competitorRaceDataGrid.Columns["BestLap"] != null)
                    competitorRaceDataGrid.Columns["BestLap"].Visible = false;
                if (competitorRaceDataGrid.Columns["LastLap"] != null)
                    competitorRaceDataGrid.Columns["LastLap"].Visible = false;
                if (competitorRaceDataGrid.Columns["currentPlace"] != null)
                {
                    competitorRaceDataGrid.Columns["currentPlace"].DisplayIndex = 0;
                    competitorRaceDataGrid.Columns["currentPlace"].Width = 70;
                }

                if (passingsDataGrid.Columns["CompetitorRace"] != null)
                    passingsDataGrid.Columns["CompetitorRace"].Visible = false;
                if (passingsDataGrid.Columns["competitorID"] != null)
                    passingsDataGrid.Columns["competitorID"].Visible = false;
                if (passingsDataGrid.Columns["DateTime"] != null)
                    passingsDataGrid.Columns["DateTime"].DefaultCellStyle.Format = "MM/dd/yyyy HH:mm:ss.fff";
                //passingsDataGrid.Columns["firstName"].Visible = false;
                //passingsDataGrid.Columns["lastName"].Visible = false;
                if (passingsDataGrid.Columns["Time"] != null)
                    passingsDataGrid.Columns["Time"].Visible = false;

                competitorRaceDataGrid_SelectionChanged(null, null);
            }
            else
            {
                competitorRaceDataGrid.DataSource = new SortableBindingList<CompetitorRace>();
            }
        }

        private void competitorRaceDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (competitorRaceDataGrid.SelectedCells != null && competitorRaceDataGrid.SelectedCells.Count > 0)
            {
                int index = competitorRaceDataGrid.SelectedCells[0].RowIndex;
                BindingList<PassingsInfo> CRpassings =
                    (competitorRaceDataGrid.DataSource as SortableBindingList<CompetitorRace>)[index].passings;

                passingsDataGrid.DataSource = CRpassings;
            }
            else
            {
                passingsDataGrid.DataSource = new BindingList<PassingsInfo>();
            }
        }
    }
}
