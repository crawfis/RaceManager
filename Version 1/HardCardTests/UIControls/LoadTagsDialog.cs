using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UIControls
{
    public partial class LoadTagsDialog : Form
    {
        public String EncryptedFileStr
        {
            get { return encryptedFileTextBox.Text; }
        }

        public String KeyFileStr
        {
            get { return keyFileTextBox.Text; }
        }

        public String CustomerID
        {
            get { return  customerNumBox.Text; }
        }

        public LoadTagsDialog()
        {
            InitializeComponent();

            this.CancelButton = cancelButton;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;

            loadEncryptedTagFileButton.Click += new EventHandler(loadEncryptedTagFileButton_Click);
            loadEncryptedKeyFileButton.Click += new EventHandler(loadEncryptedKeyFileButton_Click);
        }

        private void loadEncryptedKeyFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Title = "Select Encrypted File";
            //fd.Filter = "bin files (*.bin)|*.bin";
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                keyFileTextBox.Text = fd.FileName;
            }
            fd.Dispose();

        }

        private void loadEncryptedTagFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Title = "Select Encrypted File";
            //fd.Filter = "bin files (*.bin)|*.bin";
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                encryptedFileTextBox.Text = fd.FileName;
            }
            fd.Dispose();            
        }

    }
}
