namespace caro.client.ui_components
{
    partial class PlayerCard
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
            picAvatar = new PictureBox();
            lblPlayerName = new Label();
            lblTime = new Label();
            ((System.ComponentModel.ISupportInitialize)picAvatar).BeginInit();
            SuspendLayout();
            // 
            // picAvatar
            // 
            picAvatar.BackColor = Color.FromArgb(58, 58, 80);
            picAvatar.Location = new Point(0, 0);
            picAvatar.Name = "picAvatar";
            picAvatar.Size = new Size(70, 79);
            picAvatar.SizeMode = PictureBoxSizeMode.StretchImage;
            picAvatar.TabIndex = 0;
            picAvatar.TabStop = false;
            // 
            // lblPlayerName
            // 
            lblPlayerName.AutoSize = true;
            lblPlayerName.Font = new Font("Segoe UI", 11F);
            lblPlayerName.ForeColor = Color.White;
            lblPlayerName.Location = new Point(76, 9);
            lblPlayerName.Name = "lblPlayerName";
            lblPlayerName.Size = new Size(49, 20);
            lblPlayerName.TabIndex = 1;
            lblPlayerName.Text = "Player";
            // 
            // lblTime
            // 
            lblTime.AutoSize = true;
            lblTime.Font = new Font("Segoe UI", 11F);
            lblTime.ForeColor = Color.DeepSkyBlue;
            lblTime.Location = new Point(76, 41);
            lblTime.Name = "lblTime";
            lblTime.Size = new Size(71, 20);
            lblTime.TabIndex = 2;
            lblTime.Text = "Time: 20s";
            lblTime.Click += lblTime_Click;
            // 
            // PlayerCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(42, 42, 60);
            Controls.Add(lblTime);
            Controls.Add(lblPlayerName);
            Controls.Add(picAvatar);
            ForeColor = Color.White;
            Name = "PlayerCard";
            Size = new Size(156, 79);
            ((System.ComponentModel.ISupportInitialize)picAvatar).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox picAvatar;
        private Label lblPlayerName;
        private Label lblTime;
    }
}
