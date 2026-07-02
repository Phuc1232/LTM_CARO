namespace caro.client
{
    partial class Home
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            btnPlayOnline = new caro.client.ui_components.MenuButton();
            btnPlayWithAI = new caro.client.ui_components.MenuButton();
            btnMatchHistory = new caro.client.ui_components.MenuButton();
            btnLogOut = new caro.client.ui_components.MenuButton();
            btnRanking = new caro.client.ui_components.MenuButton();
            lblUsername = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.FromArgb(128, 255, 255);
            label1.Font = new Font("Segoe UI", 36F);
            label1.ForeColor = Color.Black;
            label1.Location = new Point(342, 58);
            label1.Name = "label1";
            label1.Size = new Size(298, 65);
            label1.TabIndex = 5;
            label1.Text = "CARO GAME";
            // 
            // btnPlayOnline
            // 
            btnPlayOnline.BackColor = Color.FromArgb(30, 30, 46);
            btnPlayOnline.ForeColor = SystemColors.ControlText;
            btnPlayOnline.HoverBackColor = Color.LightGray;
            btnPlayOnline.HoverForeColor = Color.Black;
            btnPlayOnline.Location = new Point(361, 159);
            btnPlayOnline.Name = "btnPlayOnline";
            btnPlayOnline.Size = new Size(257, 47);
            btnPlayOnline.TabIndex = 0;
            btnPlayOnline.Text = "Play Online";
            btnPlayOnline.Load += btnPlayOnline_Load;
            btnPlayOnline.Click += btnPlayOnline_Click;
            // 
            // btnPlayWithAI
            // 
            btnPlayWithAI.HoverBackColor = Color.LightGray;
            btnPlayWithAI.HoverForeColor = Color.Black;
            btnPlayWithAI.Location = new Point(361, 217);
            btnPlayWithAI.Name = "btnPlayWithAI";
            btnPlayWithAI.Size = new Size(257, 47);
            btnPlayWithAI.TabIndex = 1;
            btnPlayWithAI.Text = "Play With AI";
            btnPlayWithAI.Load += btnPlayWithAI_Load;
            // 
            // btnMatchHistory
            // 
            btnMatchHistory.HoverBackColor = Color.LightGray;
            btnMatchHistory.HoverForeColor = Color.Black;
            btnMatchHistory.Location = new Point(361, 275);
            btnMatchHistory.Name = "btnMatchHistory";
            btnMatchHistory.Size = new Size(257, 47);
            btnMatchHistory.TabIndex = 2;
            btnMatchHistory.Text = "Match History";
            btnMatchHistory.Click += btnMatchHistory_Click;
            // 
            // btnLogOut
            // 
            btnLogOut.HoverBackColor = Color.LightGray;
            btnLogOut.HoverForeColor = Color.Black;
            btnLogOut.Location = new Point(361, 381);
            btnLogOut.Name = "btnLogOut";
            btnLogOut.Size = new Size(257, 47);
            btnLogOut.TabIndex = 4;
            btnLogOut.Text = "Log Out";
            btnLogOut.Load += btnLogOut_Load;
            // 
            // btnRanking
            // 
            btnRanking.HoverBackColor = Color.DeepSkyBlue;
            btnRanking.HoverForeColor = Color.White;
            btnRanking.Location = new Point(361, 328);
            btnRanking.Name = "btnRanking";
            btnRanking.Size = new Size(257, 47);
            btnRanking.TabIndex = 6;
            btnRanking.Text = "Ranking";
            btnRanking.Click += btnRanking_Click;
            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.BackColor = Color.FromArgb(0, 180, 180);
            lblUsername.Font = new Font("Segoe UI", 16F);
            lblUsername.Location = new Point(12, 9);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(123, 30);
            lblUsername.TabIndex = 7;
            lblUsername.Text = "Ng??i Ch?i";
            lblUsername.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Home
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Blue;
            BackgroundImage = Properties.Resources._596b5f16_70c9_49f3_a4c4_fb18db30178f;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(979, 580);
            Controls.Add(lblUsername);
            Controls.Add(btnRanking);
            Controls.Add(label1);
            Controls.Add(btnLogOut);
            Controls.Add(btnMatchHistory);
            Controls.Add(btnPlayWithAI);
            Controls.Add(btnPlayOnline);
            ForeColor = Color.White;
            Name = "Home";
            Text = "Play Online";
            Load += Home_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ui_components.MenuButton btnPlayOnline;
        private ui_components.MenuButton btnPlayWithAI;
        private ui_components.MenuButton btnMatchHistory;
        private ui_components.MenuButton btnLogOut;
        private ui_components.MenuButton btnRanking;
        private Label lblUsername;
    }
}
