namespace WindowsFormsApp1
{
    partial class TestController
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
            this.availableApps = new System.Windows.Forms.ComboBox();
            this.deviceStatusLabel = new System.Windows.Forms.Label();
            this.deviceRefresh = new System.Windows.Forms.Button();
            this.loadProject = new System.Windows.Forms.Button();
            this.chosenSequences = new System.Windows.Forms.ListBox();
            this.addSequence = new System.Windows.Forms.Button();
            this.removeSequence = new System.Windows.Forms.Button();
            this.loadedSequences = new System.Windows.Forms.ListBox();
            this.moveSequenceDown = new System.Windows.Forms.Button();
            this.moveSequenceUp = new System.Windows.Forms.Button();
            this.runTest = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.chosenPlatformAndroid = new System.Windows.Forms.RadioButton();
            this.choosePlatformLabel = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tipLabel = new System.Windows.Forms.Label();
            this.consoleOutput = new System.Windows.Forms.TextBox();
            this.stepsStatus = new System.Windows.Forms.CheckedListBox();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // availableApps
            // 
            this.availableApps.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.availableApps.FormattingEnabled = true;
            this.availableApps.Location = new System.Drawing.Point(6, 6);
            this.availableApps.Name = "availableApps";
            this.availableApps.Size = new System.Drawing.Size(268, 21);
            this.availableApps.TabIndex = 2;
            this.availableApps.SelectionChangeCommitted += new System.EventHandler(this.comboBoxWithAvailableApps_SelectionChangeCommitted);
            // 
            // deviceStatusLabel
            // 
            this.deviceStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.deviceStatusLabel.AutoSize = true;
            this.deviceStatusLabel.Location = new System.Drawing.Point(34, 438);
            this.deviceStatusLabel.Name = "deviceStatusLabel";
            this.deviceStatusLabel.Size = new System.Drawing.Size(33, 13);
            this.deviceStatusLabel.TabIndex = 9;
            this.deviceStatusLabel.Text = "None";
            // 
            // deviceRefresh
            // 
            this.deviceRefresh.Location = new System.Drawing.Point(6, 432);
            this.deviceRefresh.Name = "deviceRefresh";
            this.deviceRefresh.Size = new System.Drawing.Size(22, 23);
            this.deviceRefresh.TabIndex = 10;
            this.deviceRefresh.Text = "R";
            this.deviceRefresh.UseVisualStyleBackColor = true;
            this.deviceRefresh.Click += new System.EventHandler(this.connectedDeviceRefreshButton_Click);
            // 
            // loadProject
            // 
            this.loadProject.Location = new System.Drawing.Point(6, 33);
            this.loadProject.Name = "loadProject";
            this.loadProject.Size = new System.Drawing.Size(268, 23);
            this.loadProject.TabIndex = 12;
            this.loadProject.Text = "Load Actions";
            this.loadProject.UseVisualStyleBackColor = true;
            this.loadProject.Click += new System.EventHandler(this.loadSequencesButton_Click);
            // 
            // chosenSequences
            // 
            this.chosenSequences.FormattingEnabled = true;
            this.chosenSequences.Location = new System.Drawing.Point(332, 71);
            this.chosenSequences.Name = "chosenSequences";
            this.chosenSequences.Size = new System.Drawing.Size(266, 381);
            this.chosenSequences.TabIndex = 13;
            // 
            // addSequence
            // 
            this.addSequence.Location = new System.Drawing.Point(280, 208);
            this.addSequence.Name = "addSequence";
            this.addSequence.Size = new System.Drawing.Size(37, 36);
            this.addSequence.TabIndex = 14;
            this.addSequence.Text = "→";
            this.addSequence.UseVisualStyleBackColor = true;
            this.addSequence.Click += new System.EventHandler(this.addSequence_Click);
            // 
            // removeSequence
            // 
            this.removeSequence.Location = new System.Drawing.Point(280, 267);
            this.removeSequence.Name = "removeSequence";
            this.removeSequence.Size = new System.Drawing.Size(37, 36);
            this.removeSequence.TabIndex = 18;
            this.removeSequence.Text = "←";
            this.removeSequence.UseVisualStyleBackColor = true;
            this.removeSequence.Click += new System.EventHandler(this.removeSequence_Click);
            // 
            // loadedSequences
            // 
            this.loadedSequences.FormattingEnabled = true;
            this.loadedSequences.Location = new System.Drawing.Point(6, 71);
            this.loadedSequences.Name = "loadedSequences";
            this.loadedSequences.Size = new System.Drawing.Size(268, 381);
            this.loadedSequences.TabIndex = 22;
            // 
            // moveSequenceDown
            // 
            this.moveSequenceDown.Location = new System.Drawing.Point(604, 267);
            this.moveSequenceDown.Name = "moveSequenceDown";
            this.moveSequenceDown.Size = new System.Drawing.Size(37, 36);
            this.moveSequenceDown.TabIndex = 24;
            this.moveSequenceDown.Text = "↓";
            this.moveSequenceDown.UseVisualStyleBackColor = true;
            this.moveSequenceDown.Click += new System.EventHandler(this.moveSequenceDown_Click);
            // 
            // moveSequenceUp
            // 
            this.moveSequenceUp.Location = new System.Drawing.Point(604, 208);
            this.moveSequenceUp.Name = "moveSequenceUp";
            this.moveSequenceUp.Size = new System.Drawing.Size(37, 36);
            this.moveSequenceUp.TabIndex = 23;
            this.moveSequenceUp.Text = "↑";
            this.moveSequenceUp.UseVisualStyleBackColor = true;
            this.moveSequenceUp.Click += new System.EventHandler(this.moveSequenceUp_Click);
            // 
            // runTest
            // 
            this.runTest.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.runTest.Location = new System.Drawing.Point(6, 32);
            this.runTest.Name = "runTest";
            this.runTest.Size = new System.Drawing.Size(212, 25);
            this.runTest.TabIndex = 0;
            this.runTest.Text = "RUN TEST";
            this.runTest.UseVisualStyleBackColor = true;
            this.runTest.Click += new System.EventHandler(this.runTestButton_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(668, 487);
            this.tabControl.TabIndex = 25;
            this.tabControl.Tag = "";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.chosenPlatformAndroid);
            this.tabPage1.Controls.Add(this.choosePlatformLabel);
            this.tabPage1.Controls.Add(this.availableApps);
            this.tabPage1.Controls.Add(this.moveSequenceDown);
            this.tabPage1.Controls.Add(this.loadProject);
            this.tabPage1.Controls.Add(this.moveSequenceUp);
            this.tabPage1.Controls.Add(this.loadedSequences);
            this.tabPage1.Controls.Add(this.removeSequence);
            this.tabPage1.Controls.Add(this.chosenSequences);
            this.tabPage1.Controls.Add(this.addSequence);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(660, 461);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Tests Setup";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // chosenPlatformAndroid
            // 
            this.chosenPlatformAndroid.AutoSize = true;
            this.chosenPlatformAndroid.Checked = true;
            this.chosenPlatformAndroid.Location = new System.Drawing.Point(332, 33);
            this.chosenPlatformAndroid.Name = "chosenPlatformAndroid";
            this.chosenPlatformAndroid.Size = new System.Drawing.Size(61, 17);
            this.chosenPlatformAndroid.TabIndex = 26;
            this.chosenPlatformAndroid.TabStop = true;
            this.chosenPlatformAndroid.Text = "Android";
            this.chosenPlatformAndroid.UseVisualStyleBackColor = true;
            this.chosenPlatformAndroid.CheckedChanged += new System.EventHandler(this.choosenPlatformAndroid_CheckedChanged);
            // 
            // choosePlatformLabel
            // 
            this.choosePlatformLabel.AutoSize = true;
            this.choosePlatformLabel.Location = new System.Drawing.Point(329, 9);
            this.choosePlatformLabel.Name = "choosePlatformLabel";
            this.choosePlatformLabel.Size = new System.Drawing.Size(93, 13);
            this.choosePlatformLabel.TabIndex = 25;
            this.choosePlatformLabel.Text = "Choosen Platform:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tipLabel);
            this.tabPage2.Controls.Add(this.consoleOutput);
            this.tabPage2.Controls.Add(this.stepsStatus);
            this.tabPage2.Controls.Add(this.deviceRefresh);
            this.tabPage2.Controls.Add(this.runTest);
            this.tabPage2.Controls.Add(this.deviceStatusLabel);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(660, 461);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Tests Execution";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tipLabel
            // 
            this.tipLabel.AutoSize = true;
            this.tipLabel.Location = new System.Drawing.Point(249, 438);
            this.tipLabel.Name = "tipLabel";
            this.tipLabel.Size = new System.Drawing.Size(405, 13);
            this.tipLabel.TabIndex = 13;
            this.tipLabel.Text = "Click on Console and press any key to pause the updating. Press any key to resume" +
    ".";
            // 
            // consoleOutput
            // 
            this.consoleOutput.Location = new System.Drawing.Point(250, 32);
            this.consoleOutput.Multiline = true;
            this.consoleOutput.Name = "consoleOutput";
            this.consoleOutput.ReadOnly = true;
            this.consoleOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.consoleOutput.Size = new System.Drawing.Size(404, 389);
            this.consoleOutput.TabIndex = 12;
            this.consoleOutput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.consoleOutput_KeyPress);
            // 
            // stepsStatus
            // 
            this.stepsStatus.Enabled = false;
            this.stepsStatus.FormattingEnabled = true;
            this.stepsStatus.Location = new System.Drawing.Point(6, 72);
            this.stepsStatus.Name = "stepsStatus";
            this.stepsStatus.Size = new System.Drawing.Size(212, 349);
            this.stepsStatus.TabIndex = 11;
            // 
            // TestController
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(694, 515);
            this.Controls.Add(this.tabControl);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(710, 554);
            this.MinimumSize = new System.Drawing.Size(710, 554);
            this.Name = "TestController";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Manual Tester";
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ComboBox availableApps;
        private System.Windows.Forms.Label deviceStatusLabel;
        private System.Windows.Forms.Button deviceRefresh;
        private System.Windows.Forms.Button loadProject;
        private System.Windows.Forms.ListBox chosenSequences;
        private System.Windows.Forms.Button addSequence;
        private System.Windows.Forms.Button removeSequence;
        private System.Windows.Forms.ListBox loadedSequences;
        private System.Windows.Forms.Button moveSequenceDown;
        private System.Windows.Forms.Button moveSequenceUp;
        private System.Windows.Forms.Button runTest;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox consoleOutput;
        private System.Windows.Forms.CheckedListBox stepsStatus;
        private System.Windows.Forms.Label tipLabel;
        private System.Windows.Forms.RadioButton chosenPlatformAndroid;
        private System.Windows.Forms.Label choosePlatformLabel;
    }
}

