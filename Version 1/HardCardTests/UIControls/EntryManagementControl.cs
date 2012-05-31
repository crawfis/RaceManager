using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UIControls
{
    public partial class EntryManagementControl : UserControl
    {
        public EntryManagementControl()
        {
            InitializeComponent();
        }

        public void SetData(String classText, String brandText, String sponsorsText)
        {
            this.classTextBox.Text = classText;
            this.brandTextBox.Text = brandText;
        }
    }
}
