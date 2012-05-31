namespace UIControls
{
    partial class EventManagementControl
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
            this.eventNameTextBox = new System.Windows.Forms.TextBox();
            this.eventCityTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.validClassesGridView = new System.Windows.Forms.DataGridView();
            this.eventsDataGridView = new System.Windows.Forms.DataGridView();
            this.activeClassesGridView = new System.Windows.Forms.DataGridView();
            this.addEventButton = new System.Windows.Forms.Button();
            this.removeEventButton = new System.Windows.Forms.Button();
            this.activeToValidClassesButton = new System.Windows.Forms.Button();
            this.validToActiveClassesButton = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.exportScheduleButton = new System.Windows.Forms.Button();
            this.importScheduleButton = new System.Windows.Forms.Button();
            this.modifyEventButton = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.sessionTypeComboBox = new System.Windows.Forms.ComboBox();
            this.addRaceButton = new System.Windows.Forms.Button();
            this.addRaceTextField = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.eventRacesGridView = new System.Windows.Forms.DataGridView();
            this.removeEventDateButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.editEventDateButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.addEventDateButton = new System.Windows.Forms.Button();
            this.eventDatesGridView = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.eventStateComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.closeDialogButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.validClassesGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.eventsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.activeClassesGridView)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eventRacesGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.eventDatesGridView)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // eventNameTextBox
            // 
            this.eventNameTextBox.Location = new System.Drawing.Point(80, 26);
            this.eventNameTextBox.Name = "eventNameTextBox";
            this.eventNameTextBox.Size = new System.Drawing.Size(296, 20);
            this.eventNameTextBox.TabIndex = 0;
            // 
            // eventCityTextBox
            // 
            this.eventCityTextBox.Location = new System.Drawing.Point(80, 52);
            this.eventCityTextBox.Name = "eventCityTextBox";
            this.eventCityTextBox.Size = new System.Drawing.Size(296, 20);
            this.eventCityTextBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Event Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(454, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "State";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "City";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(258, 315);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(124, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Valid Classes for Session";
            // 
            // validClassesGridView
            // 
            this.validClassesGridView.AllowUserToAddRows = false;
            this.validClassesGridView.AllowUserToDeleteRows = false;
            this.validClassesGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.validClassesGridView.Location = new System.Drawing.Point(261, 331);
            this.validClassesGridView.Name = "validClassesGridView";
            this.validClassesGridView.ReadOnly = true;
            this.validClassesGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.validClassesGridView.Size = new System.Drawing.Size(163, 118);
            this.validClassesGridView.TabIndex = 15;
            // 
            // eventsDataGridView
            // 
            this.eventsDataGridView.AllowUserToAddRows = false;
            this.eventsDataGridView.AllowUserToDeleteRows = false;
            this.eventsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.eventsDataGridView.Location = new System.Drawing.Point(14, 19);
            this.eventsDataGridView.Name = "eventsDataGridView";
            this.eventsDataGridView.ReadOnly = true;
            this.eventsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.eventsDataGridView.Size = new System.Drawing.Size(296, 488);
            this.eventsDataGridView.TabIndex = 16;
            this.eventsDataGridView.SelectionChanged += new System.EventHandler(this.eventsDataGridView_SelectionChanged);
            // 
            // activeClassesGridView
            // 
            this.activeClassesGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.activeClassesGridView.Location = new System.Drawing.Point(478, 200);
            this.activeClassesGridView.Name = "activeClassesGridView";
            this.activeClassesGridView.Size = new System.Drawing.Size(153, 249);
            this.activeClassesGridView.TabIndex = 17;
            // 
            // addEventButton
            // 
            this.addEventButton.Location = new System.Drawing.Point(382, 24);
            this.addEventButton.Name = "addEventButton";
            this.addEventButton.Size = new System.Drawing.Size(76, 23);
            this.addEventButton.TabIndex = 18;
            this.addEventButton.Text = "Add Event";
            this.addEventButton.UseVisualStyleBackColor = true;
            this.addEventButton.Click += new System.EventHandler(this.addEventButtonClicked);
            // 
            // removeEventButton
            // 
            this.removeEventButton.Location = new System.Drawing.Point(492, 50);
            this.removeEventButton.Name = "removeEventButton";
            this.removeEventButton.Size = new System.Drawing.Size(97, 23);
            this.removeEventButton.TabIndex = 19;
            this.removeEventButton.Text = "Remove Event";
            this.removeEventButton.UseVisualStyleBackColor = true;
            this.removeEventButton.Click += new System.EventHandler(this.removeEventButton_Click);
            // 
            // activeToValidClassesButton
            // 
            this.activeToValidClassesButton.Location = new System.Drawing.Point(432, 370);
            this.activeToValidClassesButton.Name = "activeToValidClassesButton";
            this.activeToValidClassesButton.Size = new System.Drawing.Size(40, 23);
            this.activeToValidClassesButton.TabIndex = 20;
            this.activeToValidClassesButton.Text = "<<";
            this.activeToValidClassesButton.UseVisualStyleBackColor = true;
            // 
            // validToActiveClassesButton
            // 
            this.validToActiveClassesButton.Location = new System.Drawing.Point(432, 399);
            this.validToActiveClassesButton.Name = "validToActiveClassesButton";
            this.validToActiveClassesButton.Size = new System.Drawing.Size(40, 23);
            this.validToActiveClassesButton.TabIndex = 21;
            this.validToActiveClassesButton.Text = ">>";
            this.validToActiveClassesButton.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(475, 171);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(76, 13);
            this.label9.TabIndex = 22;
            this.label9.Text = "Active Classes";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.exportScheduleButton);
            this.groupBox1.Controls.Add(this.importScheduleButton);
            this.groupBox1.Controls.Add(this.modifyEventButton);
            this.groupBox1.Controls.Add(this.removeEventButton);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.addEventButton);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.sessionTypeComboBox);
            this.groupBox1.Controls.Add(this.addRaceButton);
            this.groupBox1.Controls.Add(this.addRaceTextField);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.eventRacesGridView);
            this.groupBox1.Controls.Add(this.eventCityTextBox);
            this.groupBox1.Controls.Add(this.removeEventDateButton);
            this.groupBox1.Controls.Add(this.eventNameTextBox);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.editEventDateButton);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.addEventDateButton);
            this.groupBox1.Controls.Add(this.activeClassesGridView);
            this.groupBox1.Controls.Add(this.eventDatesGridView);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.validToActiveClassesButton);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.activeToValidClassesButton);
            this.groupBox1.Controls.Add(this.eventStateComboBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.validClassesGridView);
            this.groupBox1.Location = new System.Drawing.Point(324, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(643, 521);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Event Details";
            // 
            // exportScheduleButton
            // 
            this.exportScheduleButton.Location = new System.Drawing.Point(144, 455);
            this.exportScheduleButton.Name = "exportScheduleButton";
            this.exportScheduleButton.Size = new System.Drawing.Size(108, 23);
            this.exportScheduleButton.TabIndex = 43;
            this.exportScheduleButton.Text = "Export Schedule";
            this.exportScheduleButton.UseVisualStyleBackColor = true;
            // 
            // importScheduleButton
            // 
            this.importScheduleButton.Location = new System.Drawing.Point(24, 455);
            this.importScheduleButton.Name = "importScheduleButton";
            this.importScheduleButton.Size = new System.Drawing.Size(109, 23);
            this.importScheduleButton.TabIndex = 42;
            this.importScheduleButton.Text = "Import Schedule";
            this.importScheduleButton.UseVisualStyleBackColor = true;
            // 
            // modifyEventButton
            // 
            this.modifyEventButton.Location = new System.Drawing.Point(464, 24);
            this.modifyEventButton.Name = "modifyEventButton";
            this.modifyEventButton.Size = new System.Drawing.Size(127, 23);
            this.modifyEventButton.TabIndex = 27;
            this.modifyEventButton.Text = "Save Event Changes";
            this.modifyEventButton.UseVisualStyleBackColor = true;
            this.modifyEventButton.Click += new System.EventHandler(this.modifyEventButton_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(264, 184);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(73, 13);
            this.label11.TabIndex = 41;
            this.label11.Text = "Session name";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(266, 233);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(67, 13);
            this.label10.TabIndex = 40;
            this.label10.Text = "Session type";
            // 
            // sessionTypeComboBox
            // 
            this.sessionTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sessionTypeComboBox.FormattingEnabled = true;
            this.sessionTypeComboBox.Items.AddRange(new object[] {
            "Practice",
            "Qualifying",
            "Race"});
            this.sessionTypeComboBox.Location = new System.Drawing.Point(259, 249);
            this.sessionTypeComboBox.Name = "sessionTypeComboBox";
            this.sessionTypeComboBox.Size = new System.Drawing.Size(130, 21);
            this.sessionTypeComboBox.TabIndex = 39;
            // 
            // addRaceButton
            // 
            this.addRaceButton.Location = new System.Drawing.Point(258, 276);
            this.addRaceButton.Name = "addRaceButton";
            this.addRaceButton.Size = new System.Drawing.Size(103, 23);
            this.addRaceButton.TabIndex = 38;
            this.addRaceButton.Text = "Add New Session";
            this.addRaceButton.UseVisualStyleBackColor = true;
            // 
            // addRaceTextField
            // 
            this.addRaceTextField.Location = new System.Drawing.Point(259, 200);
            this.addRaceTextField.Name = "addRaceTextField";
            this.addRaceTextField.Size = new System.Drawing.Size(117, 20);
            this.addRaceTextField.TabIndex = 28;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(462, 184);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(169, 13);
            this.label7.TabIndex = 36;
            this.label7.Text = "(use DEL to delete, type to modify)";
            // 
            // eventRacesGridView
            // 
            this.eventRacesGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.eventRacesGridView.Location = new System.Drawing.Point(24, 200);
            this.eventRacesGridView.Name = "eventRacesGridView";
            this.eventRacesGridView.Size = new System.Drawing.Size(228, 249);
            this.eventRacesGridView.TabIndex = 37;
            // 
            // removeEventDateButton
            // 
            this.removeEventDateButton.Location = new System.Drawing.Point(376, 142);
            this.removeEventDateButton.Name = "removeEventDateButton";
            this.removeEventDateButton.Size = new System.Drawing.Size(96, 23);
            this.removeEventDateButton.TabIndex = 32;
            this.removeEventDateButton.Text = "Remove Date";
            this.removeEventDateButton.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 184);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 34;
            this.label5.Text = "Schedule";
            // 
            // editEventDateButton
            // 
            this.editEventDateButton.Location = new System.Drawing.Point(376, 113);
            this.editEventDateButton.Name = "editEventDateButton";
            this.editEventDateButton.Size = new System.Drawing.Size(96, 23);
            this.editEventDateButton.TabIndex = 31;
            this.editEventDateButton.Text = "Edit Date";
            this.editEventDateButton.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(77, 184);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(169, 13);
            this.label6.TabIndex = 35;
            this.label6.Text = "(use DEL to delete, type to modify)";
            // 
            // addEventDateButton
            // 
            this.addEventDateButton.Location = new System.Drawing.Point(376, 84);
            this.addEventDateButton.Name = "addEventDateButton";
            this.addEventDateButton.Size = new System.Drawing.Size(96, 23);
            this.addEventDateButton.TabIndex = 30;
            this.addEventDateButton.Text = "Add Date";
            this.addEventDateButton.UseVisualStyleBackColor = true;
            // 
            // eventDatesGridView
            // 
            this.eventDatesGridView.AllowUserToAddRows = false;
            this.eventDatesGridView.AllowUserToDeleteRows = false;
            this.eventDatesGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.eventDatesGridView.Location = new System.Drawing.Point(80, 84);
            this.eventDatesGridView.Name = "eventDatesGridView";
            this.eventDatesGridView.ReadOnly = true;
            this.eventDatesGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.eventDatesGridView.Size = new System.Drawing.Size(290, 81);
            this.eventDatesGridView.TabIndex = 29;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "Date(s)";
            // 
            // eventStateComboBox
            // 
            this.eventStateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.eventStateComboBox.FormattingEnabled = true;
            this.eventStateComboBox.Items.AddRange(new object[] {
            "AL",
            "AK",
            "AS",
            "AZ",
            "AR",
            "CA",
            "CO",
            "CT",
            "DE",
            "DC",
            "FM",
            "FL",
            "GA",
            "GU",
            "HI",
            "ID",
            "IL",
            "IN",
            "IA",
            "KS",
            "KY",
            "LA",
            "ME",
            "MH",
            "MD",
            "MA",
            "MI",
            "MN",
            "MS",
            "MO",
            "MT",
            "NE",
            "NV",
            "NH",
            "NJ",
            "NM",
            "NY",
            "NC",
            "ND",
            "MP",
            "OH",
            "OK",
            "OR",
            "PW",
            "PA",
            "PR",
            "RI",
            "SC",
            "SD",
            "TN",
            "TX",
            "UT",
            "VT",
            "VI",
            "VA",
            "WA",
            "WV",
            "WI",
            "WY"});
            this.eventStateComboBox.Location = new System.Drawing.Point(393, 52);
            this.eventStateComboBox.Name = "eventStateComboBox";
            this.eventStateComboBox.Size = new System.Drawing.Size(55, 21);
            this.eventStateComboBox.TabIndex = 3;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.eventsDataGridView);
            this.groupBox2.Location = new System.Drawing.Point(3, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(315, 521);
            this.groupBox2.TabIndex = 26;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Events";
            // 
            // closeDialogButton
            // 
            this.closeDialogButton.Location = new System.Drawing.Point(1144, 578);
            this.closeDialogButton.Name = "closeDialogButton";
            this.closeDialogButton.Size = new System.Drawing.Size(127, 23);
            this.closeDialogButton.TabIndex = 42;
            this.closeDialogButton.Text = "Close";
            this.closeDialogButton.UseVisualStyleBackColor = true;
            // 
            // EventManagementControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.closeDialogButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "EventManagementControl";
            this.Size = new System.Drawing.Size(1033, 544);
            ((System.ComponentModel.ISupportInitialize)(this.validClassesGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.eventsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.activeClassesGridView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eventRacesGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.eventDatesGridView)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox eventNameTextBox;
        private System.Windows.Forms.TextBox eventCityTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DataGridView validClassesGridView;
        private System.Windows.Forms.DataGridView eventsDataGridView;
        private System.Windows.Forms.DataGridView activeClassesGridView;
        private System.Windows.Forms.Button addEventButton;
        private System.Windows.Forms.Button removeEventButton;
        private System.Windows.Forms.Button activeToValidClassesButton;
        private System.Windows.Forms.Button validToActiveClassesButton;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button modifyEventButton;
        private System.Windows.Forms.ComboBox eventStateComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView eventDatesGridView;
        private System.Windows.Forms.Button removeEventDateButton;
        private System.Windows.Forms.Button editEventDateButton;
        private System.Windows.Forms.Button addEventDateButton;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView eventRacesGridView;
        private System.Windows.Forms.Button addRaceButton;
        private System.Windows.Forms.TextBox addRaceTextField;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox sessionTypeComboBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button closeDialogButton;
        private System.Windows.Forms.Button exportScheduleButton;
        private System.Windows.Forms.Button importScheduleButton;
    }
}
