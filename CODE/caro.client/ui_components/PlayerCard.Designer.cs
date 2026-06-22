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
            picAvatar.Margin = new Padding(3, 4, 3, 4);
            picAvatar.Name = "picAvatar";
            picAvatar.Size = new Size(80, 105);
            picAvatar.SizeMode = PictureBoxSizeMode.StretchImage;
            picAvatar.TabIndex = 0;
            picAvatar.TabStop = false;
            // 
            // lblPlayerName
            // 
            lblPlayerName.AutoSize = true;
            lblPlayerName.Font = new Font("Segoe UI", 11F);
            lblPlayerName.ForeColor = Color.White;
            lblPlayerName.Location = new Point(87, 12);
            lblPlayerName.Name = "lblPlayerName";
            lblPlayerName.Size = new Size(64, 25);
            lblPlayerName.TabIndex = 1;
            lblPlayerName.Text = "Player";
            // 
            // lblTime
            // 
            lblTime.AutoSize = true;
            lblTime.Font = new Font("Segoe UI", 11F);
            lblTime.ForeColor = Color.DeepSkyBlue;
            lblTime.Location = new Point(87, 55);
            lblTime.Name = "lblTime";
            lblTime.Size = new Size(90, 25);
            lblTime.TabIndex = 2;
            lblTime.Text = "Time: 20s";
            lblTime.Click += lblTime_Click;
            // 
            // PlayerCard
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(42, 42, 60);
            Controls.Add(lblTime);
            Controls.Add(lblPlayerName);
            Controls.Add(picAvatar);
            ForeColor = Color.White;
            Margin = new Padding(3, 4, 3, 4);
            Name = "PlayerCard";
            Size = new Size(178, 111);
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
