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
            picAvatar.Click += picAvatar_Click;
            // 
            // lblPlayerName
            // 
            lblPlayerName.AutoSize = true;
            lblPlayerName.Font = new Font("Segoe UI", 11F);
            lblPlayerName.ForeColor = Color.Black;
            lblPlayerName.Location = new Point(15, 15);
            lblPlayerName.Name = "lblPlayerName";
            lblPlayerName.Size = new Size(49, 20);
            lblPlayerName.TabIndex = 1;
            lblPlayerName.Text = "Player";
            // 
            // lblTime
            // 
            lblTime.AutoSize = true;
            lblTime.Font = new Font("Segoe UI", 11F);
            lblTime.ForeColor = Color.DarkCyan;
            lblTime.Location = new Point(15, 45);
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
            BackColor = Color.FromArgb(128, 255, 255);
            Controls.Add(lblTime);
            Controls.Add(lblPlayerName);
            Controls.Add(picAvatar);
            ForeColor = Color.Black;
            Name = "PlayerCard";
            Size = new Size(220, 70);
            Load += PlayerCard_Load;
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
