namespace UIControls
{
    partial class RaceInformationControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.raceInformationGridView = new System.Windows.Forms.DataGridView();
            this.eventComboBox = new System.Windows.Forms.ComboBox();
            this.startCollectingDataButton = new System.Windows.Forms.Button();
            this.stopCollectingDataButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.selectedRaceComboBox = new System.Windows.Forms.ComboBox();
            this.passingsDataGrid = new System.Windows.Forms.DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.exportPassingsButton = new System.Windows.Forms.Button();
            this.exportRaceInfoButton = new System.Windows.Forms.Button();
            this.addPassingButton = new System.Windows.Forms.Button();
            this.removePassingButton = new System.Windows.Forms.Button();
            this.undeletePassingButton = new System.Windows.Forms.Button();
            this.enableAutoScrollButton = new System.Windows.Forms.Button();
            this.disableAutoscrollButton = new System.Windows.Forms.Button();
            this.sortByClassCheckBox = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.firstPassingLapOneCheckbox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.raceInformationGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.passingsDataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // raceInformationGridView
            // 
            this.raceInformationGridView.AllowUserToAddRows = false;
            this.raceInformationGridView.AllowUserToDeleteRows = false;
            this.raceInformationGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.raceInformationGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.raceInformationGridView.Location = new System.Drawing.Point(0, 13);
            this.raceInformationGridView.Name = "raceInformationGridView";
            this.raceInformationGridView.ReadOnly = true;
            this.raceInformationGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.raceInformationGridView.Size = new System.Drawing.Size(910, 307);
            this.raceInformationGridView.TabIndex = 0;
            // 
            // eventComboBox
            // 
            this.eventComboBox.FormattingEnabled = true;
            this.eventComboBox.Location = new System.Drawing.Point(920, 26);
            this.eventComboBox.Name = "eventComboBox";
            this.eventComboBox.Size = new System.Drawing.Size(131, 21);
            this.eventComboBox.TabIndex = 1;
            // 
            // startCollectingDataButton
            // 
            this.startCollectingDataButton.Location = new System.Drawing.Point(920, 109);
            this.startCollectingDataButton.Name = "startCollectingDataButton";
            this.startCollectingDataButton.Size = new System.Drawing.Size(131, 23);
            this.startCollectingDataButton.TabIndex = 4;
            this.startCollectingDataButton.Text = "Start Run";
            this.startCollectingDataButton.UseVisualStyleBackColor = true;
            // 
            // stopCollectingDataButton
            // 
            this.stopCollectingDataButton.Location = new System.Drawing.Point(920, 138);
            this.stopCollectingDataButton.Name = "stopCollectingDataButton";
            this.stopCollectingDataButton.Size = new System.Drawing.Size(131, 23);
            this.stopCollectingDataButton.TabIndex = 5;
            this.stopCollectingDataButton.Text = "Stop Run";
            this.stopCollectingDataButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(917, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Current Event";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Standings";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(920, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(27, 13);
            this.label5.TabIndex = 30;
            this.label5.Text = "Run";
            // 
            // selectedRaceComboBox
            // 
            this.selectedRaceComboBox.FormattingEnabled = true;
            this.selectedRaceComboBox.Location = new System.Drawing.Point(920, 66);
            this.selectedRaceComboBox.Name = "selectedRaceComboBox";
            this.selectedRaceComboBox.Size = new System.Drawing.Size(131, 21);
            this.selectedRaceComboBox.TabIndex = 29;
            // 
            // passingsDataGrid
            // 
            this.passingsDataGrid.AllowUserToAddRows = false;
            this.passingsDataGrid.AllowUserToDeleteRows = false;
            this.passingsDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.passingsDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.passingsDataGrid.Location = new System.Drawing.Point(0, 13);
            this.passingsDataGrid.Name = "passingsDataGrid";
            this.passingsDataGrid.ReadOnly = true;
            this.passingsDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.passingsDataGrid.Size = new System.Drawing.Size(910, 314);
            this.passingsDataGrid.TabIndex = 31;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(243, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "Passings (Double Click to Bring Competitor Dialog)";
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Left;
            this.splitContainer1.Location = new System.Drawing.Point(0, 15);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.passingsDataGrid);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.raceInformationGridView);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Size = new System.Drawing.Size(914, 665);
            this.splitContainer1.SplitterDistance = 331;
            this.splitContainer1.SplitterWidth = 10;
            this.splitContainer1.TabIndex = 33;
            // 
            // exportPassingsButton
            // 
            this.exportPassingsButton.Location = new System.Drawing.Point(920, 469);
            this.exportPassingsButton.Name = "exportPassingsButton";
            this.exportPassingsButton.Size = new System.Drawing.Size(131, 23);
            this.exportPassingsButton.TabIndex = 34;
            this.exportPassingsButton.Text = "Export Passings";
            this.exportPassingsButton.UseVisualStyleBackColor = true;
            // 
            // exportRaceInfoButton
            // 
            this.exportRaceInfoButton.Location = new System.Drawing.Point(920, 498);
            this.exportRaceInfoButton.Name = "exportRaceInfoButton";
            this.exportRaceInfoButton.Size = new System.Drawing.Size(131, 23);
            this.exportRaceInfoButton.TabIndex = 35;
            this.exportRaceInfoButton.Text = "Export Result Info";
            this.exportRaceInfoButton.UseVisualStyleBackColor = true;
            // 
            // addPassingButton
            // 
            this.addPassingButton.Location = new System.Drawing.Point(920, 193);
            this.addPassingButton.Name = "addPassingButton";
            this.addPassingButton.Size = new System.Drawing.Size(131, 23);
            this.addPassingButton.TabIndex = 36;
            this.addPassingButton.Text = "Add Passing";
            this.addPassingButton.UseVisualStyleBackColor = true;
            // 
            // removePassingButton
            // 
            this.removePassingButton.Location = new System.Drawing.Point(920, 222);
            this.removePassingButton.Name = "removePassingButton";
            this.removePassingButton.Size = new System.Drawing.Size(131, 23);
            this.removePassingButton.TabIndex = 37;
            this.removePassingButton.Text = "Delete Passing";
            this.removePassingButton.UseVisualStyleBackColor = true;
            // 
            // undeletePassingButton
            // 
            this.undeletePassingButton.Location = new System.Drawing.Point(920, 251);
            this.undeletePassingButton.Name = "undeletePassingButton";
            this.undeletePassingButton.Size = new System.Drawing.Size(131, 23);
            this.undeletePassingButton.TabIndex = 38;
            this.undeletePassingButton.Text = "Undelete Passing";
            this.undeletePassingButton.UseVisualStyleBackColor = true;
            // 
            // enableAutoScrollButton
            // 
            this.enableAutoScrollButton.Location = new System.Drawing.Point(920, 393);
            this.enableAutoScrollButton.Name = "enableAutoScrollButton";
            this.enableAutoScrollButton.Size = new System.Drawing.Size(131, 23);
            this.enableAutoScrollButton.TabIndex = 39;
            this.enableAutoScrollButton.Text = "Enable Autoscroll";
            this.enableAutoScrollButton.UseVisualStyleBackColor = true;
            // 
            // disableAutoscrollButton
            // 
            this.disableAutoscrollButton.Location = new System.Drawing.Point(920, 422);
            this.disableAutoscrollButton.Name = "disableAutoscrollButton";
            this.disableAutoscrollButton.Size = new System.Drawing.Size(131, 23);
            this.disableAutoscrollButton.TabIndex = 40;
            this.disableAutoscrollButton.Text = "Disable Autoscroll Button";
            this.disableAutoscrollButton.UseVisualStyleBackColor = true;
            // 
            // sortByClassCheckBox
            // 
            this.sortByClassCheckBox.AutoSize = true;
            this.sortByClassCheckBox.Location = new System.Drawing.Point(920, 533);
            this.sortByClassCheckBox.Name = "sortByClassCheckBox";
            this.sortByClassCheckBox.Size = new System.Drawing.Size(110, 17);
            this.sortByClassCheckBox.TabIndex = 41;
            this.sortByClassCheckBox.Text = "Sort First By Class";
            this.sortByClassCheckBox.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::UIControls.Properties.Resources.Hardcardlogo1;
            this.pictureBox1.Location = new System.Drawing.Point(976, 605);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(75, 75);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 42;
            this.pictureBox1.TabStop = false;
            // 
            // firstPassingLapOneCheckbox
            // 
            this.firstPassingLapOneCheckbox.AutoSize = true;
            this.firstPassingLapOneCheckbox.Location = new System.Drawing.Point(920, 556);
            this.firstPassingLapOneCheckbox.Name = "firstPassingLapOneCheckbox";
            this.firstPassingLapOneCheckbox.Size = new System.Drawing.Size(139, 17);
            this.firstPassingLapOneCheckbox.TabIndex = 43;
            this.firstPassingLapOneCheckbox.Text = "First Passing is Lap One";
            this.firstPassingLapOneCheckbox.UseVisualStyleBackColor = true;
            // 
            // RaceInformationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.firstPassingLapOneCheckbox);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.sortByClassCheckBox);
            this.Controls.Add(this.disableAutoscrollButton);
            this.Controls.Add(this.enableAutoScrollButton);
            this.Controls.Add(this.undeletePassingButton);
            this.Controls.Add(this.removePassingButton);
            this.Controls.Add(this.addPassingButton);
            this.Controls.Add(this.exportRaceInfoButton);
            this.Controls.Add(this.exportPassingsButton);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.selectedRaceComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.stopCollectingDataButton);
            this.Controls.Add(this.startCollectingDataButton);
            this.Controls.Add(this.eventComboBox);
            this.Name = "RaceInformationControl";
            this.Padding = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.Size = new System.Drawing.Size(1061, 680);
            ((System.ComponentModel.ISupportInitialize)(this.raceInformationGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.passingsDataGrid)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView raceInformationGridView;
        private System.Windows.Forms.ComboBox eventComboBox;
        private System.Windows.Forms.Button startCollectingDataButton;
        private System.Windows.Forms.Button stopCollectingDataButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox selectedRaceComboBox;
        private System.Windows.Forms.DataGridView passingsDataGrid;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button exportPassingsButton;
        private System.Windows.Forms.Button exportRaceInfoButton;
        private System.Windows.Forms.Button addPassingButton;
        private System.Windows.Forms.Button removePassingButton;
        private System.Windows.Forms.Button undeletePassingButton;
        private System.Windows.Forms.Button enableAutoScrollButton;
        private System.Windows.Forms.Button disableAutoscrollButton;
        private System.Windows.Forms.CheckBox sortByClassCheckBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox firstPassingLapOneCheckbox;
    }
}
