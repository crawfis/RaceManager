using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Hardcard.Scoring;

namespace UIControls
{
    public partial class DataGridControl : UserControl
    {
        public DataGridView DataGridView
        {
            get { return dataGridView; }
            set { dataGridView = value; }
        }
        
        public DataGridControl()
        {
            InitializeComponent();
        }

        public void HideColumns(List<String> columnsToHide)
        {
            if (dataGridView.Columns == null) return;

            foreach (String columnName in columnsToHide)
            {
                dataGridView.Columns[columnName].Visible = false;
            }
        }

        public void ShowColumns(List<String> columnsToShow)
        {
            if (dataGridView.Columns == null) return;

            foreach (String columnName in columnsToShow)
            {
                dataGridView.Columns[columnName].Visible = true;
            }
        }
    }
}
