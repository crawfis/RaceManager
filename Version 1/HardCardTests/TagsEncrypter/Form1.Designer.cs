namespace TagsEncrypter
{
    partial class Form1
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
            this.tagFileStringTextBox = new System.Windows.Forms.TextBox();
            this.selectTagFileButton = new System.Windows.Forms.Button();
            this.selectEncryptedFileButton = new System.Windows.Forms.Button();
            this.encryptFileButton = new System.Windows.Forms.Button();
            this.selectKeyFileButton = new System.Windows.Forms.Button();
            this.encryptStringTextField = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.encryptedFileTextField = new System.Windows.Forms.TextBox();
            this.keyFileTextField = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.userSettingsStringTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tagFileStringTextBox
            // 
            this.tagFileStringTextBox.Location = new System.Drawing.Point(12, 12);
            this.tagFileStringTextBox.Name = "tagFileStringTextBox";
            this.tagFileStringTextBox.Size = new System.Drawing.Size(329, 20);
            this.tagFileStringTextBox.TabIndex = 0;
            // 
            // selectTagFileButton
            // 
            this.selectTagFileButton.Location = new System.Drawing.Point(349, 10);
            this.selectTagFileButton.Name = "selectTagFileButton";
            this.selectTagFileButton.Size = new System.Drawing.Size(119, 23);
            this.selectTagFileButton.TabIndex = 1;
            this.selectTagFileButton.Text = "Select Tag File";
            this.selectTagFileButton.UseVisualStyleBackColor = true;
            // 
            // selectEncryptedFileButton
            // 
            this.selectEncryptedFileButton.Location = new System.Drawing.Point(349, 50);
            this.selectEncryptedFileButton.Name = "selectEncryptedFileButton";
            this.selectEncryptedFileButton.Size = new System.Drawing.Size(119, 23);
            this.selectEncryptedFileButton.TabIndex = 2;
            this.selectEncryptedFileButton.Text = "Select Encrypted File";
            this.selectEncryptedFileButton.UseVisualStyleBackColor = true;
            // 
            // encryptFileButton
            // 
            this.encryptFileButton.Location = new System.Drawing.Point(349, 185);
            this.encryptFileButton.Name = "encryptFileButton";
            this.encryptFileButton.Size = new System.Drawing.Size(119, 23);
            this.encryptFileButton.TabIndex = 3;
            this.encryptFileButton.Text = "Encrypt the File";
            this.encryptFileButton.UseVisualStyleBackColor = true;
            // 
            // selectKeyFileButton
            // 
            this.selectKeyFileButton.Location = new System.Drawing.Point(349, 79);
            this.selectKeyFileButton.Name = "selectKeyFileButton";
            this.selectKeyFileButton.Size = new System.Drawing.Size(119, 23);
            this.selectKeyFileButton.TabIndex = 4;
            this.selectKeyFileButton.Text = "Select Key File";
            this.selectKeyFileButton.UseVisualStyleBackColor = true;
            // 
            // encryptStringTextField
            // 
            this.encryptStringTextField.Location = new System.Drawing.Point(118, 187);
            this.encryptStringTextField.Name = "encryptStringTextField";
            this.encryptStringTextField.Size = new System.Drawing.Size(223, 20);
            this.encryptStringTextField.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 190);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Encryption String";
            // 
            // encryptedFileTextField
            // 
            this.encryptedFileTextField.Location = new System.Drawing.Point(12, 53);
            this.encryptedFileTextField.Name = "encryptedFileTextField";
            this.encryptedFileTextField.Size = new System.Drawing.Size(329, 20);
            this.encryptedFileTextField.TabIndex = 7;
            // 
            // keyFileTextField
            // 
            this.keyFileTextField.Location = new System.Drawing.Point(12, 79);
            this.keyFileTextField.Name = "keyFileTextField";
            this.keyFileTextField.Size = new System.Drawing.Size(329, 20);
            this.keyFileTextField.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 164);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "User Settings String";
            // 
            // userSettingsStringTextBox
            // 
            this.userSettingsStringTextBox.Location = new System.Drawing.Point(118, 161);
            this.userSettingsStringTextBox.Name = "userSettingsStringTextBox";
            this.userSettingsStringTextBox.Size = new System.Drawing.Size(223, 20);
            this.userSettingsStringTextBox.TabIndex = 10;
            this.userSettingsStringTextBox.Text = "default";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 219);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.userSettingsStringTextBox);
            this.Controls.Add(this.keyFileTextField);
            this.Controls.Add(this.encryptedFileTextField);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.encryptStringTextField);
            this.Controls.Add(this.selectKeyFileButton);
            this.Controls.Add(this.encryptFileButton);
            this.Controls.Add(this.selectEncryptedFileButton);
            this.Controls.Add(this.selectTagFileButton);
            this.Controls.Add(this.tagFileStringTextBox);
            this.Name = "Form1";
            this.Text = "Tags Encryption";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tagFileStringTextBox;
        private System.Windows.Forms.Button selectTagFileButton;
        private System.Windows.Forms.Button selectEncryptedFileButton;
        private System.Windows.Forms.Button encryptFileButton;
        private System.Windows.Forms.Button selectKeyFileButton;
        private System.Windows.Forms.TextBox encryptStringTextField;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox encryptedFileTextField;
        private System.Windows.Forms.TextBox keyFileTextField;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox userSettingsStringTextBox;
    }
}

