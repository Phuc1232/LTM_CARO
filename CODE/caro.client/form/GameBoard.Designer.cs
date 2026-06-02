namespace caro.client.form
{
    partial class GameBoard
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
            boardControl1 = new caro.client.ui_components.BoardControl();
            panel1 = new Panel();
            menuButton6 = new caro.client.ui_components.MenuButton();
            menuButton5 = new caro.client.ui_components.MenuButton();
            menuButton4 = new caro.client.ui_components.MenuButton();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // boardControl1
            // 
            boardControl1.BackColor = Color.FromArgb(42, 42, 60);
            boardControl1.BorderStyle = BorderStyle.FixedSingle;
            boardControl1.Location = new Point(331, 139);
            boardControl1.Name = "boardControl1";
            boardControl1.Size = new Size(608, 610);
            boardControl1.TabIndex = 2;
            // 
            // panel1
            // 
            panel1.Controls.Add(menuButton6);
            panel1.Controls.Add(menuButton5);
            panel1.Controls.Add(menuButton4);
            panel1.Location = new Point(61, 513);
            panel1.Name = "panel1";
            panel1.Size = new Size(176, 206);
            panel1.TabIndex = 3;
            // 
            // menuButton6
            // 
            menuButton6.HoverBackColor = Color.DeepSkyBlue;
            menuButton6.HoverForeColor = Color.White;
            menuButton6.Location = new Point(20, 150);
            menuButton6.Name = "menuButton6";
            menuButton6.Size = new Size(141, 33);
            menuButton6.TabIndex = 6;
            menuButton6.Text = "menuButton6";
            // 
            // menuButton5
            // 
            menuButton5.HoverBackColor = Color.DeepSkyBlue;
            menuButton5.HoverForeColor = Color.White;
            menuButton5.Location = new Point(20, 91);
            menuButton5.Name = "menuButton5";
            menuButton5.Size = new Size(141, 33);
            menuButton5.TabIndex = 5;
            menuButton5.Text = "menuButton5";
            // 
            // menuButton4
            // 
            menuButton4.BackColor = Color.FromArgb(88, 101, 242);
            menuButton4.HoverBackColor = Color.DeepSkyBlue;
            menuButton4.HoverForeColor = Color.White;
            menuButton4.Location = new Point(20, 34);
            menuButton4.Name = "menuButton4";
            menuButton4.Size = new Size(141, 33);
            menuButton4.TabIndex = 4;
            menuButton4.Text = "menuButton4";
            // 
            // GameBoard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 46);
            ClientSize = new Size(1184, 761);
            Controls.Add(panel1);
            Controls.Add(boardControl1);
            Name = "GameBoard";
            Text = "GameBoard";
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private ui_components.BoardControl boardControl1;
        private Panel panel1;
        private ui_components.MenuButton menuButton3;
        private ui_components.MenuButton menuButton2;
        private ui_components.MenuButton menuButton1;
        private ui_components.MenuButton menuButton6;
        private ui_components.MenuButton menuButton5;
        private ui_components.MenuButton menuButton4;
    }
}