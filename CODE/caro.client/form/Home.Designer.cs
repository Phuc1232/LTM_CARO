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
            menuButton1 = new caro.client.ui_components.MenuButton();
            menuButton2 = new caro.client.ui_components.MenuButton();
            menuButton3 = new caro.client.ui_components.MenuButton();
            menuButton5 = new caro.client.ui_components.MenuButton();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.FromArgb(30, 30, 46);
            label1.Font = new Font("Segoe UI", 36F);
            label1.ForeColor = Color.White;
            label1.Location = new Point(389, 69);
            label1.Name = "label1";
            label1.Size = new Size(298, 65);
            label1.TabIndex = 5;
            label1.Text = "CARO GAME";
            // 
            // menuButton1
            // 
            menuButton1.ForeColor = Color.FromArgb(30, 30, 46);
            menuButton1.HoverBackColor = Color.LightGray;
            menuButton1.HoverForeColor = Color.Black;
            menuButton1.Location = new Point(413, 162);
            menuButton1.Name = "menuButton1";
            menuButton1.Size = new Size(262, 52);
            menuButton1.TabIndex = 0;
            menuButton1.Text = "Play Online";
            menuButton1.Load += menuButton1_Load;
            menuButton1.Click += menuButton1_Click;
            // 
            // menuButton2
            // 
            menuButton2.HoverBackColor = Color.LightGray;
            menuButton2.HoverForeColor = Color.Black;
            menuButton2.Location = new Point(413, 220);
            menuButton2.Name = "menuButton2";
            menuButton2.Size = new Size(262, 52);
            menuButton2.TabIndex = 1;
            menuButton2.Text = "Play With AI";
            menuButton2.Load += menuButton2_Load;
            // 
            // menuButton3
            // 
            menuButton3.HoverBackColor = Color.LightGray;
            menuButton3.HoverForeColor = Color.Black;
            menuButton3.Location = new Point(413, 278);
            menuButton3.Name = "menuButton3";
            menuButton3.Size = new Size(262, 52);
            menuButton3.TabIndex = 2;
            menuButton3.Text = "Match History";
            // 
            // menuButton5
            // 
            menuButton5.HoverBackColor = Color.LightGray;
            menuButton5.HoverForeColor = Color.Black;
            menuButton5.Location = new Point(413, 336);
            menuButton5.Name = "menuButton5";
            menuButton5.Size = new Size(262, 52);
            menuButton5.TabIndex = 4;
            menuButton5.Text = "Log Out";
            menuButton5.Load += menuButton5_Load;
            // 
            // Home
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 46);
            BackgroundImage = Properties.Resources._596b5f16_70c9_49f3_a4c4_fb18db30178f;
            ClientSize = new Size(1097, 690);
            Controls.Add(label1);
            Controls.Add(menuButton5);
            Controls.Add(menuButton3);
            Controls.Add(menuButton2);
            Controls.Add(menuButton1);
            Name = "Home";
            Text = "Play Online";
            Load += Home_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ui_components.MenuButton menuButton1;
        private ui_components.MenuButton menuButton2;
        private ui_components.MenuButton menuButton3;
        private ui_components.MenuButton menuButton5;
    }
}
