namespace EatingWithYourEars
{
    partial class Client
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PlayPauseButton = new System.Windows.Forms.Button();
            this.NameLabel = new System.Windows.Forms.Label();
            this.TrackSpeedList = new System.Windows.Forms.ComboBox();
            this.SpeedLabel = new System.Windows.Forms.Label();
            this.NameField = new System.Windows.Forms.Label();
            this.VolumeTrackBar = new System.Windows.Forms.TrackBar();
            this.VolumeLabel = new System.Windows.Forms.Label();
            this.ChewMethod2CheckBox = new System.Windows.Forms.CheckBox();
            this.ChewMethod1CheckBox = new System.Windows.Forms.CheckBox();
            this.ChewMethod3CheckBox = new System.Windows.Forms.CheckBox();
            this.BiteMethod3CheckBox = new System.Windows.Forms.CheckBox();
            this.BiteMethod4CheckBox = new System.Windows.Forms.CheckBox();
            this.WaveGraph = new EatingWithYourEars.CustomWaveViewer();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.VolumeTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1491, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fullFileToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // fullFileToolStripMenuItem
            // 
            this.fullFileToolStripMenuItem.Checked = true;
            this.fullFileToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.fullFileToolStripMenuItem.Name = "fullFileToolStripMenuItem";
            this.fullFileToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.fullFileToolStripMenuItem.Text = "Full Graph";
            this.fullFileToolStripMenuItem.Click += new System.EventHandler(this.fullFileToolStripMenuItem_Click);
            // 
            // PlayPauseButton
            // 
            this.PlayPauseButton.Location = new System.Drawing.Point(385, 506);
            this.PlayPauseButton.Name = "PlayPauseButton";
            this.PlayPauseButton.Size = new System.Drawing.Size(83, 46);
            this.PlayPauseButton.TabIndex = 2;
            this.PlayPauseButton.Text = "Play";
            this.PlayPauseButton.UseVisualStyleBackColor = true;
            this.PlayPauseButton.Click += new System.EventHandler(this.PlayPauseButton_Click);
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(18, 504);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(69, 13);
            this.NameLabel.TabIndex = 3;
            this.NameLabel.Text = "Track Name:";
            // 
            // TrackSpeedList
            // 
            this.TrackSpeedList.Enabled = false;
            this.TrackSpeedList.FormattingEnabled = true;
            this.TrackSpeedList.Items.AddRange(new object[] {
            "0.25x",
            "0.50x",
            "1.00x",
            "1.50x",
            "2.00x"});
            this.TrackSpeedList.Location = new System.Drawing.Point(494, 531);
            this.TrackSpeedList.Name = "TrackSpeedList";
            this.TrackSpeedList.Size = new System.Drawing.Size(121, 21);
            this.TrackSpeedList.TabIndex = 4;
            this.TrackSpeedList.Text = "1.00x";
            this.TrackSpeedList.SelectedIndexChanged += new System.EventHandler(this.TrackSpeedList_SelectedIndexChanged);
            // 
            // SpeedLabel
            // 
            this.SpeedLabel.AutoSize = true;
            this.SpeedLabel.Location = new System.Drawing.Point(491, 504);
            this.SpeedLabel.Name = "SpeedLabel";
            this.SpeedLabel.Size = new System.Drawing.Size(122, 13);
            this.SpeedLabel.TabIndex = 5;
            this.SpeedLabel.Text = "Track Speed: (Disabled)";
            // 
            // NameField
            // 
            this.NameField.AutoSize = true;
            this.NameField.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameField.Location = new System.Drawing.Point(18, 529);
            this.NameField.Name = "NameField";
            this.NameField.Size = new System.Drawing.Size(39, 20);
            this.NameField.TabIndex = 6;
            this.NameField.Text = "N.A.";
            // 
            // VolumeTrackBar
            // 
            this.VolumeTrackBar.Location = new System.Drawing.Point(642, 529);
            this.VolumeTrackBar.Name = "VolumeTrackBar";
            this.VolumeTrackBar.Size = new System.Drawing.Size(221, 45);
            this.VolumeTrackBar.TabIndex = 7;
            this.VolumeTrackBar.Value = 5;
            this.VolumeTrackBar.ValueChanged += new System.EventHandler(this.VolumeTrackBar_ValueChanged);
            // 
            // VolumeLabel
            // 
            this.VolumeLabel.AutoSize = true;
            this.VolumeLabel.Location = new System.Drawing.Point(639, 504);
            this.VolumeLabel.Name = "VolumeLabel";
            this.VolumeLabel.Size = new System.Drawing.Size(45, 13);
            this.VolumeLabel.TabIndex = 8;
            this.VolumeLabel.Text = "Volume:";
            // 
            // ChewMethod2CheckBox
            // 
            this.ChewMethod2CheckBox.AutoSize = true;
            this.ChewMethod2CheckBox.Checked = true;
            this.ChewMethod2CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChewMethod2CheckBox.Location = new System.Drawing.Point(358, 472);
            this.ChewMethod2CheckBox.Name = "ChewMethod2CheckBox";
            this.ChewMethod2CheckBox.Size = new System.Drawing.Size(15, 14);
            this.ChewMethod2CheckBox.TabIndex = 9;
            this.ChewMethod2CheckBox.UseVisualStyleBackColor = true;
            this.ChewMethod2CheckBox.CheckedChanged += new System.EventHandler(this.ChewMethod2CheckBox_CheckedChanged);
            // 
            // ChewMethod1CheckBox
            // 
            this.ChewMethod1CheckBox.AutoSize = true;
            this.ChewMethod1CheckBox.Checked = true;
            this.ChewMethod1CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChewMethod1CheckBox.Location = new System.Drawing.Point(358, 452);
            this.ChewMethod1CheckBox.Name = "ChewMethod1CheckBox";
            this.ChewMethod1CheckBox.Size = new System.Drawing.Size(15, 14);
            this.ChewMethod1CheckBox.TabIndex = 10;
            this.ChewMethod1CheckBox.UseVisualStyleBackColor = true;
            this.ChewMethod1CheckBox.CheckedChanged += new System.EventHandler(this.ChewMethod1CheckBox_CheckedChanged);
            // 
            // ChewMethod3CheckBox
            // 
            this.ChewMethod3CheckBox.AutoSize = true;
            this.ChewMethod3CheckBox.Checked = true;
            this.ChewMethod3CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChewMethod3CheckBox.Location = new System.Drawing.Point(669, 452);
            this.ChewMethod3CheckBox.Name = "ChewMethod3CheckBox";
            this.ChewMethod3CheckBox.Size = new System.Drawing.Size(15, 14);
            this.ChewMethod3CheckBox.TabIndex = 11;
            this.ChewMethod3CheckBox.UseVisualStyleBackColor = true;
            this.ChewMethod3CheckBox.CheckedChanged += new System.EventHandler(this.ChewMethod3CheckBox_CheckedChanged);
            // 
            // BiteMethod3CheckBox
            // 
            this.BiteMethod3CheckBox.AutoSize = true;
            this.BiteMethod3CheckBox.Checked = true;
            this.BiteMethod3CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.BiteMethod3CheckBox.Location = new System.Drawing.Point(669, 472);
            this.BiteMethod3CheckBox.Name = "BiteMethod3CheckBox";
            this.BiteMethod3CheckBox.Size = new System.Drawing.Size(15, 14);
            this.BiteMethod3CheckBox.TabIndex = 12;
            this.BiteMethod3CheckBox.UseVisualStyleBackColor = true;
            this.BiteMethod3CheckBox.CheckedChanged += new System.EventHandler(this.BiteMethod3CheckBox_CheckedChanged);
            // 
            // BiteMethod4CheckBox
            // 
            this.BiteMethod4CheckBox.AutoSize = true;
            this.BiteMethod4CheckBox.Checked = true;
            this.BiteMethod4CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.BiteMethod4CheckBox.Location = new System.Drawing.Point(1161, 472);
            this.BiteMethod4CheckBox.Name = "BiteMethod4CheckBox";
            this.BiteMethod4CheckBox.Size = new System.Drawing.Size(15, 14);
            this.BiteMethod4CheckBox.TabIndex = 13;
            this.BiteMethod4CheckBox.UseVisualStyleBackColor = true;
            this.BiteMethod4CheckBox.CheckedChanged += new System.EventHandler(this.BiteMethod4CheckBox_CheckedChanged);
            // 
            // WaveGraph
            // 
            this.WaveGraph.Dock = System.Windows.Forms.DockStyle.Top;
            this.WaveGraph.Location = new System.Drawing.Point(0, 24);
            this.WaveGraph.Name = "WaveGraph";
            this.WaveGraph.SamplesPerPixel = 128;
            this.WaveGraph.Size = new System.Drawing.Size(1491, 464);
            this.WaveGraph.StartPosition = ((long)(0));
            this.WaveGraph.TabIndex = 1;
            this.WaveGraph.WaveStream = null;
            // 
            // Client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1491, 560);
            this.Controls.Add(this.BiteMethod4CheckBox);
            this.Controls.Add(this.BiteMethod3CheckBox);
            this.Controls.Add(this.ChewMethod3CheckBox);
            this.Controls.Add(this.ChewMethod1CheckBox);
            this.Controls.Add(this.ChewMethod2CheckBox);
            this.Controls.Add(this.VolumeLabel);
            this.Controls.Add(this.VolumeTrackBar);
            this.Controls.Add(this.NameField);
            this.Controls.Add(this.SpeedLabel);
            this.Controls.Add(this.TrackSpeedList);
            this.Controls.Add(this.NameLabel);
            this.Controls.Add(this.PlayPauseButton);
            this.Controls.Add(this.WaveGraph);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Client";
            this.Text = "Eating With Your Ears";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Client_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.VolumeTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private CustomWaveViewer WaveGraph;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fullFileToolStripMenuItem;
        private System.Windows.Forms.Button PlayPauseButton;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.ComboBox TrackSpeedList;
        private System.Windows.Forms.Label SpeedLabel;
        private System.Windows.Forms.Label NameField;
        private System.Windows.Forms.TrackBar VolumeTrackBar;
        private System.Windows.Forms.Label VolumeLabel;
        private System.Windows.Forms.CheckBox ChewMethod2CheckBox;
        private System.Windows.Forms.CheckBox ChewMethod1CheckBox;
        private System.Windows.Forms.CheckBox ChewMethod3CheckBox;
        private System.Windows.Forms.CheckBox BiteMethod3CheckBox;
        private System.Windows.Forms.CheckBox BiteMethod4CheckBox;
    }
}

