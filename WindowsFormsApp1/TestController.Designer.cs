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
            this.comboBoxWithAvailableApps = new System.Windows.Forms.ComboBox();
            this.connectedDeviceIDLabel = new System.Windows.Forms.Label();
            this.connectedDeviceRefreshButton = new System.Windows.Forms.Button();
            this.loadSequencesButton = new System.Windows.Forms.Button();
            this.choosenSequenceList = new System.Windows.Forms.ListBox();
            this.addSequence = new System.Windows.Forms.Button();
            this.removeSequence = new System.Windows.Forms.Button();
            this.sequenceDefinitionsList = new System.Windows.Forms.ListBox();
            this.moveSequenceDown = new System.Windows.Forms.Button();
            this.moveSequenceUp = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tipLabel = new System.Windows.Forms.Label();
            this.consoleOutput = new System.Windows.Forms.TextBox();
            this.choosenSequenceStatusCheckedList = new System.Windows.Forms.CheckedListBox();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxWithAvailableApps
            // 
            this.comboBoxWithAvailableApps.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxWithAvailableApps.FormattingEnabled = true;
            this.comboBoxWithAvailableApps.Location = new System.Drawing.Point(6, 6);
            this.comboBoxWithAvailableApps.Name = "comboBoxWithAvailableApps";
            this.comboBoxWithAvailableApps.Size = new System.Drawing.Size(268, 21);
            this.comboBoxWithAvailableApps.TabIndex = 2;
            // 
            // connectedDeviceIDLabel
            // 
            this.connectedDeviceIDLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.connectedDeviceIDLabel.AutoSize = true;
            this.connectedDeviceIDLabel.Location = new System.Drawing.Point(34, 438);
            this.connectedDeviceIDLabel.Name = "connectedDeviceIDLabel";
            this.connectedDeviceIDLabel.Size = new System.Drawing.Size(33, 13);
            this.connectedDeviceIDLabel.TabIndex = 9;
            this.connectedDeviceIDLabel.Text = "None";
            // 
            // connectedDeviceRefreshButton
            // 
            this.connectedDeviceRefreshButton.Location = new System.Drawing.Point(6, 432);
            this.connectedDeviceRefreshButton.Name = "connectedDeviceRefreshButton";
            this.connectedDeviceRefreshButton.Size = new System.Drawing.Size(22, 23);
            this.connectedDeviceRefreshButton.TabIndex = 10;
            this.connectedDeviceRefreshButton.Text = "R";
            this.connectedDeviceRefreshButton.UseVisualStyleBackColor = true;
            this.connectedDeviceRefreshButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // loadSequencesButton
            // 
            this.loadSequencesButton.Location = new System.Drawing.Point(6, 33);
            this.loadSequencesButton.Name = "loadSequencesButton";
            this.loadSequencesButton.Size = new System.Drawing.Size(268, 23);
            this.loadSequencesButton.TabIndex = 12;
            this.loadSequencesButton.Text = "Load Actions";
            this.loadSequencesButton.UseVisualStyleBackColor = true;
            this.loadSequencesButton.Click += new System.EventHandler(this.loadSequencesButton_Click);
            // 
            // choosenSequenceList
            // 
            this.choosenSequenceList.FormattingEnabled = true;
            this.choosenSequenceList.Location = new System.Drawing.Point(332, 71);
            this.choosenSequenceList.Name = "choosenSequenceList";
            this.choosenSequenceList.Size = new System.Drawing.Size(266, 381);
            this.choosenSequenceList.TabIndex = 13;
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
            // sequenceDefinitionsList
            // 
            this.sequenceDefinitionsList.FormattingEnabled = true;
            this.sequenceDefinitionsList.Location = new System.Drawing.Point(6, 71);
            this.sequenceDefinitionsList.Name = "sequenceDefinitionsList";
            this.sequenceDefinitionsList.Size = new System.Drawing.Size(268, 381);
            this.sequenceDefinitionsList.TabIndex = 22;
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
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(6, 32);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(212, 25);
            this.button1.TabIndex = 0;
            this.button1.Text = "RUN TEST";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
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
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.comboBoxWithAvailableApps);
            this.tabPage1.Controls.Add(this.moveSequenceDown);
            this.tabPage1.Controls.Add(this.loadSequencesButton);
            this.tabPage1.Controls.Add(this.moveSequenceUp);
            this.tabPage1.Controls.Add(this.sequenceDefinitionsList);
            this.tabPage1.Controls.Add(this.removeSequence);
            this.tabPage1.Controls.Add(this.choosenSequenceList);
            this.tabPage1.Controls.Add(this.addSequence);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(660, 461);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tipLabel);
            this.tabPage2.Controls.Add(this.consoleOutput);
            this.tabPage2.Controls.Add(this.choosenSequenceStatusCheckedList);
            this.tabPage2.Controls.Add(this.connectedDeviceRefreshButton);
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Controls.Add(this.connectedDeviceIDLabel);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(660, 461);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
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
            // choosenSequenceStatusCheckedList
            // 
            this.choosenSequenceStatusCheckedList.Enabled = false;
            this.choosenSequenceStatusCheckedList.FormattingEnabled = true;
            this.choosenSequenceStatusCheckedList.Location = new System.Drawing.Point(6, 72);
            this.choosenSequenceStatusCheckedList.Name = "choosenSequenceStatusCheckedList";
            this.choosenSequenceStatusCheckedList.Size = new System.Drawing.Size(212, 349);
            this.choosenSequenceStatusCheckedList.TabIndex = 11;
            this.choosenSequenceStatusCheckedList.Click += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // TestController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 509);
            this.Controls.Add(this.tabControl);
            this.Name = "TestController";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ComboBox comboBoxWithAvailableApps;
        private System.Windows.Forms.Label connectedDeviceIDLabel;
        private System.Windows.Forms.Button connectedDeviceRefreshButton;
        private System.Windows.Forms.Button loadSequencesButton;
        private System.Windows.Forms.ListBox choosenSequenceList;
        private System.Windows.Forms.Button addSequence;
        private System.Windows.Forms.Button removeSequence;
        private System.Windows.Forms.ListBox sequenceDefinitionsList;
        private System.Windows.Forms.Button moveSequenceDown;
        private System.Windows.Forms.Button moveSequenceUp;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox consoleOutput;
        private System.Windows.Forms.CheckedListBox choosenSequenceStatusCheckedList;
        private System.Windows.Forms.Label tipLabel;
    }
}

