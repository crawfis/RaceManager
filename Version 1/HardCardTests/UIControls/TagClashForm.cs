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

namespace UIControls
{
    public partial class TagClashForm : Form
    {        
        private List<CompetitorRace> crs;
        private List<TagId> tagClashFormList;
        private TagId tagID;
        private Race currentRace;
        private DataGridView passingsGrid;
        private Dictionary<TagId, CompetitorRace> disambiuationCRDict;
        
        public TagClashForm()
        {
            InitializeComponent();
            this.CancelButton = this.cancelButton;

            this.cancelButton.Click += new EventHandler(cancelButton_Click);
            this.okButton.Click += new EventHandler(okButton_Click);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.dataGridView1.SelectedRows.Count > 0)
                {
                    int index = this.dataGridView1.SelectedRows[0].Index;
                    currentRace.ResetPassings(crs[index]);
                    currentRace.ReassociatePassings(crs[index]);
                    (passingsGrid.DataSource as BindingList<PassingsInfo>).ResetBindings();
                    if (!disambiuationCRDict.Keys.Contains(tagID))
                        disambiuationCRDict.Add(tagID, crs[index]);
                    else
                    {
                        disambiuationCRDict.Remove(tagID);
                        disambiuationCRDict.Add(tagID, crs[index]);
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.StackTrace);
                DataManager.Log(exc.StackTrace);
            }

            this.tagClashFormList.Remove(this.tagID);
            this.Dispose();            
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.tagClashFormList.Remove(this.tagID);
            this.Dispose();
        }

        public void SetCompetitors(List<CompetitorRace> crs, PassingsInfo pi, List<TagId> tagClashFormList, 
            Race currentRace, DataGridView passingsGrid, Dictionary<TagId, CompetitorRace> disambiuationCRDict)
        {
            this.crs = crs;
            this.tagClashFormList = tagClashFormList;
            this.tagID = pi.ID;
            this.currentRace = currentRace;
            this.passingsGrid = passingsGrid;
            this.disambiuationCRDict = disambiuationCRDict;

            DataTable t = new DataTable();
            t.Columns.Add("First Name");
            t.Columns.Add("Last Name");
            t.Columns.Add("Tag ID");

            foreach (CompetitorRace cr in crs)
            {
                t.Rows.Add(new object[] { cr.firstName, cr.lastName, pi.ID });
                this.dataGridView1.DataSource = t;
            }
            this.dataGridView1.ClearSelection();
        }
    }
}
