namespace caro.client.form
{
    partial class MatchMaking
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
            label1 = new Label();
            lstOnlinePlayers = new ListBox();
            btnChallenge = new caro.client.ui_components.MenuButton();
            btnBack = new caro.client.ui_components.MenuButton();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.FromArgb(30, 30, 46);
            label1.Font = new Font("Segoe UI", 36F);
            label1.ForeColor = Color.White;
            label1.Location = new Point(518, 21);
            label1.Name = "label1";
            label1.Size = new Size(383, 65);
            label1.TabIndex = 6;
            label1.Text = "MATCH MAKING";
            // 
            // lstOnlinePlayers
            // 
            lstOnlinePlayers.BackColor = Color.FromArgb(42, 42, 60);
            lstOnlinePlayers.Font = new Font("Segoe UI", 20F);
            lstOnlinePlayers.ForeColor = Color.White;
            lstOnlinePlayers.FormattingEnabled = true;
            lstOnlinePlayers.Location = new Point(559, 108);
            lstOnlinePlayers.Name = "lstOnlinePlayers";
            lstOnlinePlayers.Size = new Size(300, 374);
            lstOnlinePlayers.TabIndex = 7;
            // 
            // btnChallenge
            // 
            btnChallenge.HoverBackColor = Color.DeepSkyBlue;
            btnChallenge.HoverForeColor = Color.White;
            btnChallenge.Location = new Point(587, 521);
            btnChallenge.Name = "btnChallenge";
            btnChallenge.Size = new Size(250, 60);
            btnChallenge.TabIndex = 8;
            btnChallenge.Text = "Thách đấu";
            btnChallenge.Click += btnChallenge_Click_1;
            // 
            // btnBack
            // 
            btnBack.HoverBackColor = Color.DeepSkyBlue;
            btnBack.HoverForeColor = Color.White;
            btnBack.Location = new Point(587, 601);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(250, 60);
            btnBack.TabIndex = 9;
            btnBack.Text = "Quay lại";
            // 
            // MatchMaking
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 46);
            ClientSize = new Size(1402, 690);
            Controls.Add(btnBack);
            Controls.Add(btnChallenge);
            Controls.Add(lstOnlinePlayers);
            Controls.Add(label1);
            Name = "MatchMaking";
            Text = "MatchMaking";
            Load += MatchMaking_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ListBox lstOnlinePlayers;
        private ui_components.MenuButton btnChallenge;
        private ui_components.MenuButton btnBack;
    }
}