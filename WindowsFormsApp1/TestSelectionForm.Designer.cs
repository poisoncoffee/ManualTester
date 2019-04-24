namespace WindowsFormsApp1
{
    partial class TestSelectionForm
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
            this.connectedDeviceLabel = new System.Windows.Forms.Label();
            this.connectedDeviceIDLabel = new System.Windows.Forms.Label();
            this.connectedDeviceRefreshButton = new System.Windows.Forms.Button();
            this.loadSequencesButton = new System.Windows.Forms.Button();
            this.selectedList = new System.Windows.Forms.ListBox();
            this.addSequence = new System.Windows.Forms.Button();
            this.removeSequence = new System.Windows.Forms.Button();
            this.sequencesList = new System.Windows.Forms.ListBox();
            this.moveSequenceDown = new System.Windows.Forms.Button();
            this.moveSequenceUp = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comboBoxWithAvailableApps
            // 
            this.comboBoxWithAvailableApps.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxWithAvailableApps.FormattingEnabled = true;
            this.comboBoxWithAvailableApps.Location = new System.Drawing.Point(28, 31);
            this.comboBoxWithAvailableApps.Name = "comboBoxWithAvailableApps";
            this.comboBoxWithAvailableApps.Size = new System.Drawing.Size(274, 21);
            this.comboBoxWithAvailableApps.TabIndex = 2;
            // 
            // connectedDeviceLabel
            // 
            this.connectedDeviceLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.connectedDeviceLabel.AutoSize = true;
            this.connectedDeviceLabel.Location = new System.Drawing.Point(382, 72);
            this.connectedDeviceLabel.Name = "connectedDeviceLabel";
            this.connectedDeviceLabel.Size = new System.Drawing.Size(97, 13);
            this.connectedDeviceLabel.TabIndex = 4;
            this.connectedDeviceLabel.Text = "Connected device:";
            this.connectedDeviceLabel.Click += new System.EventHandler(this.label1_Click);
            // 
            // connectedDeviceIDLabel
            // 
            this.connectedDeviceIDLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.connectedDeviceIDLabel.AutoSize = true;
            this.connectedDeviceIDLabel.Location = new System.Drawing.Point(485, 72);
            this.connectedDeviceIDLabel.Name = "connectedDeviceIDLabel";
            this.connectedDeviceIDLabel.Size = new System.Drawing.Size(33, 13);
            this.connectedDeviceIDLabel.TabIndex = 9;
            this.connectedDeviceIDLabel.Text = "None";
            // 
            // connectedDeviceRefreshButton
            // 
            this.connectedDeviceRefreshButton.Location = new System.Drawing.Point(354, 66);
            this.connectedDeviceRefreshButton.Name = "connectedDeviceRefreshButton";
            this.connectedDeviceRefreshButton.Size = new System.Drawing.Size(22, 23);
            this.connectedDeviceRefreshButton.TabIndex = 10;
            this.connectedDeviceRefreshButton.Text = "R";
            this.connectedDeviceRefreshButton.UseVisualStyleBackColor = true;
            this.connectedDeviceRefreshButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // loadSequencesButton
            // 
            this.loadSequencesButton.Location = new System.Drawing.Point(28, 67);
            this.loadSequencesButton.Name = "loadSequencesButton";
            this.loadSequencesButton.Size = new System.Drawing.Size(273, 23);
            this.loadSequencesButton.TabIndex = 12;
            this.loadSequencesButton.Text = "Load Actions";
            this.loadSequencesButton.UseVisualStyleBackColor = true;
            this.loadSequencesButton.Click += new System.EventHandler(this.loadSequencesButton_Click);
            // 
            // selectedList
            // 
            this.selectedList.FormattingEnabled = true;
            this.selectedList.Location = new System.Drawing.Point(350, 116);
            this.selectedList.Name = "selectedList";
            this.selectedList.Size = new System.Drawing.Size(266, 303);
            this.selectedList.TabIndex = 13;
            // 
            // addSequence
            // 
            this.addSequence.Location = new System.Drawing.Point(307, 216);
            this.addSequence.Name = "addSequence";
            this.addSequence.Size = new System.Drawing.Size(37, 36);
            this.addSequence.TabIndex = 14;
            this.addSequence.Text = "→";
            this.addSequence.UseVisualStyleBackColor = true;
            this.addSequence.Click += new System.EventHandler(this.addSequence_Click);
            // 
            // removeSequence
            // 
            this.removeSequence.Location = new System.Drawing.Point(307, 275);
            this.removeSequence.Name = "removeSequence";
            this.removeSequence.Size = new System.Drawing.Size(37, 36);
            this.removeSequence.TabIndex = 18;
            this.removeSequence.Text = "←";
            this.removeSequence.UseVisualStyleBackColor = true;
            this.removeSequence.Click += new System.EventHandler(this.removeSequence_Click);
            // 
            // sequencesList
            // 
            this.sequencesList.FormattingEnabled = true;
            this.sequencesList.Location = new System.Drawing.Point(33, 116);
            this.sequencesList.Name = "sequencesList";
            this.sequencesList.Size = new System.Drawing.Size(268, 303);
            this.sequencesList.TabIndex = 22;
            // 
            // moveSequenceDown
            // 
            this.moveSequenceDown.Location = new System.Drawing.Point(622, 275);
            this.moveSequenceDown.Name = "moveSequenceDown";
            this.moveSequenceDown.Size = new System.Drawing.Size(37, 36);
            this.moveSequenceDown.TabIndex = 24;
            this.moveSequenceDown.Text = "↓";
            this.moveSequenceDown.UseVisualStyleBackColor = true;
            this.moveSequenceDown.Click += new System.EventHandler(this.moveSequenceDown_Click);
            // 
            // moveSequenceUp
            // 
            this.moveSequenceUp.Location = new System.Drawing.Point(622, 216);
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
            this.button1.Location = new System.Drawing.Point(478, 437);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(139, 51);
            this.button1.TabIndex = 0;
            this.button1.Text = "RUN TEST";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // TestSelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 511);
            this.Controls.Add(this.moveSequenceDown);
            this.Controls.Add(this.moveSequenceUp);
            this.Controls.Add(this.sequencesList);
            this.Controls.Add(this.removeSequence);
            this.Controls.Add(this.addSequence);
            this.Controls.Add(this.selectedList);
            this.Controls.Add(this.loadSequencesButton);
            this.Controls.Add(this.connectedDeviceRefreshButton);
            this.Controls.Add(this.connectedDeviceIDLabel);
            this.Controls.Add(this.connectedDeviceLabel);
            this.Controls.Add(this.comboBoxWithAvailableApps);
            this.Controls.Add(this.button1);
            this.Name = "TestSelectionForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox comboBoxWithAvailableApps;
        private System.Windows.Forms.Label connectedDeviceLabel;
        private System.Windows.Forms.Label connectedDeviceIDLabel;
        private System.Windows.Forms.Button connectedDeviceRefreshButton;
        private System.Windows.Forms.Button loadSequencesButton;
        private System.Windows.Forms.ListBox selectedList;
        private System.Windows.Forms.Button addSequence;
        private System.Windows.Forms.Button removeSequence;
        private System.Windows.Forms.ListBox sequencesList;
        private System.Windows.Forms.Button moveSequenceDown;
        private System.Windows.Forms.Button moveSequenceUp;
        private System.Windows.Forms.Button button1;
    }
}

