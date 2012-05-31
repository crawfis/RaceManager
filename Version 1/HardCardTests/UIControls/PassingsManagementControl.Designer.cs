namespace UIControls
{
    partial class PassingsManagementControl
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.passingsDataGrid = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.competitorRaceDataGrid = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.undeletePassingButton = new System.Windows.Forms.Button();
            this.removePassingButton = new System.Windows.Forms.Button();
            this.addPassingButton = new System.Windows.Forms.Button();
            this.currentRaceLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.passingsDataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.competitorRaceDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Left;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.passingsDataGrid);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.competitorRaceDataGrid);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Size = new System.Drawing.Size(1024, 680);
            this.splitContainer1.SplitterDistance = 372;
            this.splitContainer1.SplitterWidth = 10;
            this.splitContainer1.TabIndex = 0;
            // 
            // passingsDataGrid
            // 
            this.passingsDataGrid.AllowUserToAddRows = false;
            this.passingsDataGrid.AllowUserToDeleteRows = false;
            this.passingsDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.passingsDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.passingsDataGrid.Location = new System.Drawing.Point(0, 13);
            this.passingsDataGrid.MultiSelect = false;
            this.passingsDataGrid.Name = "passingsDataGrid";
            this.passingsDataGrid.ReadOnly = true;
            this.passingsDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.passingsDataGrid.Size = new System.Drawing.Size(1020, 355);
            this.passingsDataGrid.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(191, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Passings for Selected Competitor Race";
            // 
            // competitorRaceDataGrid
            // 
            this.competitorRaceDataGrid.AllowUserToAddRows = false;
            this.competitorRaceDataGrid.AllowUserToDeleteRows = false;
            this.competitorRaceDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.competitorRaceDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.competitorRaceDataGrid.Location = new System.Drawing.Point(0, 13);
            this.competitorRaceDataGrid.MultiSelect = false;
            this.competitorRaceDataGrid.Name = "competitorRaceDataGrid";
            this.competitorRaceDataGrid.ReadOnly = true;
            this.competitorRaceDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.competitorRaceDataGrid.Size = new System.Drawing.Size(1020, 281);
            this.competitorRaceDataGrid.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Standings";
            // 
            // undeletePassingButton
            // 
            this.undeletePassingButton.Location = new System.Drawing.Point(1030, 250);
            this.undeletePassingButton.Name = "undeletePassingButton";
            this.undeletePassingButton.Size = new System.Drawing.Size(131, 23);
            this.undeletePassingButton.TabIndex = 41;
            this.undeletePassingButton.Text = "Undelete Passing";
            this.undeletePassingButton.UseVisualStyleBackColor = true;
            // 
            // removePassingButton
            // 
            this.removePassingButton.Location = new System.Drawing.Point(1030, 221);
            this.removePassingButton.Name = "removePassingButton";
            this.removePassingButton.Size = new System.Drawing.Size(131, 23);
            this.removePassingButton.TabIndex = 40;
            this.removePassingButton.Text = "Delete Passing";
            this.removePassingButton.UseVisualStyleBackColor = true;
            // 
            // addPassingButton
            // 
            this.addPassingButton.Location = new System.Drawing.Point(1030, 192);
            this.addPassingButton.Name = "addPassingButton";
            this.addPassingButton.Size = new System.Drawing.Size(131, 23);
            this.addPassingButton.TabIndex = 39;
            this.addPassingButton.Text = "Add Passing";
            this.addPassingButton.UseVisualStyleBackColor = true;
            // 
            // currentRaceLabel
            // 
            this.currentRaceLabel.AutoSize = true;
            this.currentRaceLabel.Location = new System.Drawing.Point(1030, 163);
            this.currentRaceLabel.Name = "currentRaceLabel";
            this.currentRaceLabel.Size = new System.Drawing.Size(0, 13);
            this.currentRaceLabel.TabIndex = 42;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1027, 150);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 43;
            this.label3.Text = "Selected Run:";
            // 
            // PassingsManagementControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.currentRaceLabel);
            this.Controls.Add(this.undeletePassingButton);
            this.Controls.Add(this.removePassingButton);
            this.Controls.Add(this.addPassingButton);
            this.Controls.Add(this.splitContainer1);
            this.Name = "PassingsManagementControl";
            this.Size = new System.Drawing.Size(1180, 680);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.passingsDataGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.competitorRaceDataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView competitorRaceDataGrid;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView passingsDataGrid;
        private System.Windows.Forms.Button undeletePassingButton;
        private System.Windows.Forms.Button removePassingButton;
        private System.Windows.Forms.Button addPassingButton;
        private System.Windows.Forms.Label currentRaceLabel;
        private System.Windows.Forms.Label label3;
    }
}
