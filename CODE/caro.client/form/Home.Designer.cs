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
            menuButton1 = new caro.client.ui_components.MenuButton();
            menuButton2 = new caro.client.ui_components.MenuButton();
            menuButton3 = new caro.client.ui_components.MenuButton();
            menuButton4 = new caro.client.ui_components.MenuButton();
            SuspendLayout();
            // 
            // menuButton1
            // 
            menuButton1.Location = new Point(280, 87);
            menuButton1.Name = "menuButton1";
            menuButton1.Size = new Size(250, 60);
            menuButton1.TabIndex = 0;
            menuButton1.Text = "Play Online";
            // 
            // menuButton2
            // 
            menuButton2.Location = new Point(280, 153);
            menuButton2.Name = "menuButton2";
            menuButton2.Size = new Size(250, 60);
            menuButton2.TabIndex = 1;
            menuButton2.Text = "Play With AI";
            // 
            // menuButton3
            // 
            menuButton3.Location = new Point(280, 219);
            menuButton3.Name = "menuButton3";
            menuButton3.Size = new Size(250, 60);
            menuButton3.TabIndex = 2;
            menuButton3.Text = "Match History";
            // 
            // menuButton4
            // 
            menuButton4.Location = new Point(280, 285);
            menuButton4.Name = "menuButton4";
            menuButton4.Size = new Size(250, 60);
            menuButton4.TabIndex = 3;
            menuButton4.Text = "Log Out";
            // 
            // Home
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(menuButton4);
            Controls.Add(menuButton3);
            Controls.Add(menuButton2);
            Controls.Add(menuButton1);
            Name = "Home";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private ui_components.MenuButton menuButton1;
        private ui_components.MenuButton menuButton2;
        private ui_components.MenuButton menuButton3;
        private ui_components.MenuButton menuButton4;
    }
}
