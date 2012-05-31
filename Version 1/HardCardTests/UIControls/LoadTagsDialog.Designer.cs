namespace UIControls
{
    partial class LoadTagsDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.encryptedFileTextBox = new System.Windows.Forms.TextBox();
            this.keyFileTextBox = new System.Windows.Forms.TextBox();
            this.loadEncryptedTagFileButton = new System.Windows.Forms.Button();
            this.loadEncryptedKeyFileButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.customerNumBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // encryptedFileTextBox
            // 
            this.encryptedFileTextBox.Location = new System.Drawing.Point(12, 12);
            this.encryptedFileTextBox.Name = "encryptedFileTextBox";
            this.encryptedFileTextBox.Size = new System.Drawing.Size(283, 20);
            this.encryptedFileTextBox.TabIndex = 0;
            // 
            // keyFileTextBox
            // 
            this.keyFileTextBox.Location = new System.Drawing.Point(12, 38);
            this.keyFileTextBox.Name = "keyFileTextBox";
            this.keyFileTextBox.Size = new System.Drawing.Size(283, 20);
            this.keyFileTextBox.TabIndex = 1;
            // 
            // loadEncryptedTagFileButton
            // 
            this.loadEncryptedTagFileButton.Location = new System.Drawing.Point(301, 9);
            this.loadEncryptedTagFileButton.Name = "loadEncryptedTagFileButton";
            this.loadEncryptedTagFileButton.Size = new System.Drawing.Size(136, 23);
            this.loadEncryptedTagFileButton.TabIndex = 2;
            this.loadEncryptedTagFileButton.Text = "Load Encrypted Tags";
            this.loadEncryptedTagFileButton.UseVisualStyleBackColor = true;
            // 
            // loadEncryptedKeyFileButton
            // 
            this.loadEncryptedKeyFileButton.Location = new System.Drawing.Point(301, 38);
            this.loadEncryptedKeyFileButton.Name = "loadEncryptedKeyFileButton";
            this.loadEncryptedKeyFileButton.Size = new System.Drawing.Size(136, 23);
            this.loadEncryptedKeyFileButton.TabIndex = 3;
            this.loadEncryptedKeyFileButton.Text = "Load Key File";
            this.loadEncryptedKeyFileButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(281, 80);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(362, 80);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // customerNumBox
            // 
            this.customerNumBox.AcceptsTab = true;
            this.customerNumBox.Location = new System.Drawing.Point(109, 62);
            this.customerNumBox.Name = "customerNumBox";
            this.customerNumBox.Size = new System.Drawing.Size(100, 20);
            this.customerNumBox.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Customer Number";
            // 
            // LoadTagsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(441, 110);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.customerNumBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.loadEncryptedKeyFileButton);
            this.Controls.Add(this.loadEncryptedTagFileButton);
            this.Controls.Add(this.keyFileTextBox);
            this.Controls.Add(this.encryptedFileTextBox);
            this.Name = "LoadTagsDialog";
            this.Text = "Load Encrypted Tags ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox encryptedFileTextBox;
        private System.Windows.Forms.TextBox keyFileTextBox;
        private System.Windows.Forms.Button loadEncryptedTagFileButton;
        private System.Windows.Forms.Button loadEncryptedKeyFileButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox customerNumBox;
        private System.Windows.Forms.Label label1;
    }
}