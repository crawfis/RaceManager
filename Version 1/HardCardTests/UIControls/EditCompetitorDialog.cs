using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EventProject;
using Hardcard.Scoring;
using System.Collections;

namespace UIControls
{
    public partial class EditCompetitorDialog : Form
    {
        public Button CancelBtn
        {
            get { return cancelButton; }
            set { cancelButton = value; }
        }
        
        public EditCompetitorDialog()
        {
            InitializeComponent();
            this.Text = "Edit Competitor";
            okButton.Click += new EventHandler(okButton_Click);
            competitorDataInput.SetUseLastNameAutoComplete(true);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            //
            //1. Unknown tag info came, no competitor is originally known for the incoming tag
            //
            //possible cases: 
            //a) cancel (nothing done)
            //b) new competitor added
            //c) old competitor (existing in the competitor's list, but not racing list) 
            //   added to the race
            //
            if (competitor == null)
            {
                Competitor newCompetitor = new Competitor();
                newCompetitor.ID = DataManager.getNextID();//important! otherwise will have id = 0
                bool res = competitorDataInput.UpdateCompetitorData(ref newCompetitor);
                if (!res)
                {
                    MessageBox.Show("There was a problem with the entered data.");
                    return;
                }
                
                //Competitor existingCompetitor = DataManager.Instance.GetCompetitorByTagID(newCompetitor.TagNumber);
                //TODO: have to check as well for the name (assume the admin has entered John Doe,
                //but John Doe already exists somewhere in the competitors list, though with a different
                //or no tag. How to proceed there? Show the same warning dialog?
                Competitor existingCompetitor = null; 
                List<Competitor> existingCmps = DataManager.Instance.GetCompetitorByName(newCompetitor.FirstName, newCompetitor.LastName);
                if (existingCmps.Count > 0)
                    existingCompetitor = existingCmps[0];
                if (existingCompetitor != null)
                {
                    //AddNewCompetitor(existingCompetitor, false);
                    //this.Dispose();
                    //return;
                    
                    //TEMPORARILY COMMENTED OUT

                    Form userDialog = GetUserDialog("Warning!",
                        "There already exists a competitor with the name " +
                        existingCompetitor.FirstName + " " + existingCompetitor.LastName +
                        ". Do you want to proceed with this user or create a new one?",
                        "Proceed with current user", "Create a new one");
                    DialogResult dr = userDialog.ShowDialog();
                    if (dr == System.Windows.Forms.DialogResult.Cancel)
                    {
                        this.Dispose();
                        return;
                    }
                    else if (dr == System.Windows.Forms.DialogResult.Ignore)
                    {
                        if (newCompetitor.FirstName.Equals("") && newCompetitor.LastName.Equals(""))
                        {
                            if (MessageBox.Show("Are you sure you would like to add competitor with empty name fields?",
                                "Please confirm", MessageBoxButtons.YesNo) == DialogResult.No)
                            {
                                return;
                            }
                        }

                        AddNewCompetitor(newCompetitor);
                        this.Dispose();
                        return;
                    }
                    else if (dr == System.Windows.Forms.DialogResult.OK)//proceed
                    {
                        if (newCompetitor.FirstName.Equals("") && newCompetitor.LastName.Equals(""))
                        {
                            if (MessageBox.Show("Are you sure you would like to add competitor with empty name fields?",
                                "Please confirm", MessageBoxButtons.YesNo) == DialogResult.No)
                            {
                                return;
                            }
                        }

                        //have to overwrite tags
                        existingCompetitor.TagNumber = newCompetitor.TagNumber;
                        existingCompetitor.TagNumber2 = newCompetitor.TagNumber2;
                        
                        AddNewCompetitor(existingCompetitor, false);
                        this.Dispose();
                        return;
                    }

                    //AddNewCompetitor(newCompetitor, false);
                    //this.Dispose();
                    //return;


                }
                else //just save new competitor to the competitors list
                {
                    if (newCompetitor.FirstName.Equals("") && newCompetitor.LastName.Equals(""))
                    {
                        if (MessageBox.Show("Are you sure you would like to add competitor with empty name fields?",
                            "Please confirm", MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            return;
                        }
                    }
                    AddNewCompetitor(newCompetitor);
                    this.Dispose();
                    return;
                }
            }
            
            //
            //2. some existing competitor was originally selected in the grid
            //
            //possible cases:
            //update or cancel
            //
            if (competitor != null && haveToAddEventEntry)// && !competitorDataInput.ChangesMade(competitor))
            {
                if (MessageBox.Show("Are you sure you would like to proceed?",
                    "Confirmation Dialog", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    this.Dispose();
                }
                
                EventEntry newEventEntry = new EventEntry();
                newEventEntry.competitor = competitor;
                Event currentEvent = raceInformationControl.GetSelectedEvent();
                if (currentEvent != null)
                {
                    newEventEntry.eventID = currentEvent.ID;
                    DataManager.Instance.EventEntries.Add(newEventEntry);
                }
                CompetitorRace cr = new CompetitorRace(newEventEntry.competitorID);
                cr.tagID = competitor.TagNumber;
                cr.tagID2 = competitor.TagNumber2;
                cr.bikeNumber = competitor.BikeNumber;
                //cr.className = newCompetitor.//newEventEntry.className;
                cr.raceParent = raceInformationControl.GetCurrentSession();
                raceInformationControl.GetCurrentSession().AddCompetitorRace(cr);

                UpdatePassings();

                this.Dispose();
            }
            else if(competitor != null && !haveToAddEventEntry)
            {
                if (MessageBox.Show("Are you sure you would like to save the changes?",
                    "Confirmation Dialog", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    this.Dispose();
                }
                else
                {
                    bool updateResult = competitorDataInput.UpdateCompetitorData(ref competitor);
                    if (!updateResult)
                    {
                        MessageBox.Show("There was a problem with the entered data.");
                        return;
                    }
                    
                    UpdatePassings();

                    this.Dispose();
                }
            }
        }

        private void AddNewCompetitor(Competitor newCompetitor, bool addToCompetitorsList = true)
        {
            if(addToCompetitorsList)
                DataManager.Instance.Competitors.Add(newCompetitor);

            competitor = newCompetitor;
            EventEntry newEventEntry = new EventEntry();
            newEventEntry.competitor = newCompetitor;
            Event currentEvent = raceInformationControl.GetSelectedEvent();
            if (currentEvent != null)
            {
                newEventEntry.eventID = currentEvent.ID;
                newEventEntry.tagNumber = newCompetitor.TagNumber;
                newEventEntry.tagNumber2 = newCompetitor.TagNumber2;
                newEventEntry.bikeNumber = newCompetitor.BikeNumber;
                if (classComboBox.SelectedIndex != -1)
                {
                    newEventEntry.className = classComboBox.Items[classComboBox.SelectedIndex] as String;
                }

                DataManager.Instance.EventEntries.Add(newEventEntry);
            }
            CompetitorRace cr = new CompetitorRace(newEventEntry.competitorID);
            cr.EventEntry = newEventEntry;
            cr.lastName = newCompetitor.LastName;
            cr.firstName = newCompetitor.FirstName;
            cr.competitorID = newCompetitor.ID;
            cr.tagID = newCompetitor.TagNumber;
            cr.tagID2 = newCompetitor.TagNumber2;
            cr.bikeNumber = newCompetitor.BikeNumber;
            cr.className = newEventEntry.className;
            cr.raceParent = raceInformationControl.GetCurrentSession();
            raceInformationControl.GetCurrentSession().AddCompetitorRace(cr);

            UpdatePassings();

        }

        /// <summary>
        /// This method updates the existing PassingInfo objects with the new 
        /// competitor information, as well as sets up the hash which is used
        /// by newly incoming PassingInfo objects to find their related competitor.
        /// </summary>
        private void UpdatePassings()
        {
            raceInformationControl.RefreshPassingsGrid();
        }

        private Competitor competitor;
        private PassingsInfo pi;
        private BindingList<PassingsInfo> passingsDataList;
        private bool haveToAddEventEntry; // BUG - this is never assigned
        private RaceInformationControl raceInformationControl;
        
        /// <summary>
        /// This method checks whether there exists a competitor with the given id
        /// and loads his/her information to the dialog UI.
        /// </summary>
        public void SetCompetitorData(int competitorID, PassingsInfo pi, BindingList<PassingsInfo> passingsDataList, 
            RaceInformationControl parent)
        {
            this.pi = pi;
            this.passingsDataList = passingsDataList;
            this.raceInformationControl = parent;

            if (parent.GetCurrentRace() != null)
            {
                BindingList<String> validClasses = new BindingList<String>();
                foreach (Class cl in parent.GetCurrentRace().validClasses)
                {
                    validClasses.Add( (cl.name != null ? cl.name : "" ));
                }
                classComboBox.DataSource = validClasses;
            }

            //Console.WriteLine((raceInformationControl.RaceCompetitorsHash[pi.ID] as CompetitorRace).className);
            //String className = (raceInformationControl.RaceCompetitorsHash[pi.ID] as CompetitorRace).className;
            //if (className != null)//now only for existing competitors with event entries
            //    this.eventEntryClassTextBox.Text = className;

            foreach (Competitor c in DataManager.Instance.Competitors)
            {
                if (c.ID == competitorID)
                {
                    competitorDataInput.SetData(c);
                    competitor = c;
                    return;
                }
            }

            //if no competitor found:
            //load tag id only, populate other fields with some default values
            //may need a button that checks whether a person with a given name 
            //exists in the list of competitors - to help the admin
            competitorDataInput.SetData("", "", new Address(), new PhoneNumber(),
                DateTime.Now, 30, true, "", "", "3333", pi.ID, new TagId());
        }

        /// <summary>
        /// Creates a custom dialog.
        /// </summary>
        private Form GetUserDialog(String title, String labelText, String okButtonText, String secondButtonText)
        {
            Form customDialog = new Form();
            customDialog.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Label descriptionLabel = new Label();
            Button okButton = new Button();
            Button secondButton = new Button();
            Button cancelButton = new Button();

            customDialog.Text = title;
            descriptionLabel.Text = labelText;
            okButton.Text = okButtonText;
            secondButton.Text = secondButtonText;
            cancelButton.Text = "Cancel";
            
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            secondButton.DialogResult = System.Windows.Forms.DialogResult.Ignore;//hacky!
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            customDialog.Width = 450;
            customDialog.Height = 155;
            descriptionLabel.SetBounds(30, 20, 350, 50);
            okButton.SetBounds(30, 100, 120, 22);
            secondButton.SetBounds(160, 100, 120, 22);
            cancelButton.SetBounds(290, 100, 120, 22);

            customDialog.Controls.Add(descriptionLabel);
            customDialog.Controls.Add(okButton);
            customDialog.Controls.Add(secondButton);
            customDialog.Controls.Add(cancelButton);

            customDialog.CancelButton = cancelButton;

            return customDialog;                        
        }

    }
}
