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
            BnQuit = new caro.client.ui_components.MenuButton();
            BnNewGame = new caro.client.ui_components.MenuButton();
            btnSurrender = new caro.client.ui_components.MenuButton();
            playerCard1 = new caro.client.ui_components.PlayerCard();
            playerCard2 = new caro.client.ui_components.PlayerCard();
            chatBox1 = new caro.client.ui_components.ChatBox();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // boardControl1
            // 
            boardControl1.BackColor = Color.FromArgb(42, 42, 60);
            boardControl1.BorderStyle = BorderStyle.FixedSingle;
            boardControl1.Location = new Point(324, 37);
            boardControl1.Margin = new Padding(3, 4, 3, 4);
            boardControl1.Name = "boardControl1";
            boardControl1.Size = new Size(608, 610);
            boardControl1.TabIndex = 2;
            // 
            // panel1
            // 
            panel1.Controls.Add(BnQuit);
            panel1.Controls.Add(BnNewGame);
            panel1.Controls.Add(btnSurrender);
            panel1.Location = new Point(12, 393);
            panel1.Name = "panel1";
            panel1.Size = new Size(176, 206);
            panel1.TabIndex = 3;
            // 
            // BnQuit
            // 
            BnQuit.HoverBackColor = Color.DeepSkyBlue;
            BnQuit.HoverForeColor = Color.White;
            BnQuit.Location = new Point(20, 150);
            BnQuit.Margin = new Padding(3, 4, 3, 4);
            BnQuit.Name = "BnQuit";
            BnQuit.Size = new Size(141, 33);
            BnQuit.TabIndex = 6;
            BnQuit.Text = "Quit";
            BnQuit.Load += btnQuit_Load;
            BnQuit.Click += btnQuit_Click;
            // 
            // BnNewGame
            // 
            BnNewGame.HoverBackColor = Color.DeepSkyBlue;
            BnNewGame.HoverForeColor = Color.White;
            BnNewGame.Location = new Point(20, 91);
            BnNewGame.Margin = new Padding(3, 4, 3, 4);
            BnNewGame.Name = "BnNewGame";
            BnNewGame.Size = new Size(141, 33);
            BnNewGame.TabIndex = 5;
            BnNewGame.Text = "New Game";
            BnNewGame.Load += btnNewGame_Load;
            BnNewGame.Click += btnNewGame_Click;
            // 
            // btnSurrender
            // 
            btnSurrender.BackColor = Color.FromArgb(88, 101, 242);
            btnSurrender.HoverBackColor = Color.DeepSkyBlue;
            btnSurrender.HoverForeColor = Color.White;
            btnSurrender.Location = new Point(20, 34);
            btnSurrender.Margin = new Padding(3, 4, 3, 4);
            btnSurrender.Name = "btnSurrender";
            btnSurrender.Size = new Size(141, 33);
            btnSurrender.TabIndex = 4;
            btnSurrender.Text = "Surrender";
            btnSurrender.Load += btnSurrender_Load;
            btnSurrender.Click += btnSurrender_Click;
            // 
            // playerCard1
            // 
            playerCard1.Avatar = null;
            playerCard1.BackColor = Color.FromArgb(42, 42, 60);
            playerCard1.ForeColor = Color.White;
            playerCard1.Location = new Point(32, 53);
            playerCard1.Margin = new Padding(3, 4, 3, 4);
            playerCard1.Name = "playerCard1";
            playerCard1.PlayerName = "Player";
            playerCard1.Size = new Size(156, 79);
            playerCard1.TabIndex = 4;
            playerCard1.TimeText = "Time: 20s";
            // 
            // playerCard2
            // 
            playerCard2.Avatar = null;
            playerCard2.BackColor = Color.FromArgb(42, 42, 60);
            playerCard2.ForeColor = Color.White;
            playerCard2.Location = new Point(32, 169);
            playerCard2.Margin = new Padding(3, 4, 3, 4);
            playerCard2.Name = "playerCard2";
            playerCard2.PlayerName = "Player";
            playerCard2.Size = new Size(156, 79);
            playerCard2.TabIndex = 5;
            playerCard2.TimeText = "Time: 20s";
            // 
            // chatBox1
            // 
            chatBox1.BackColor = Color.FromArgb(42, 42, 60);
            chatBox1.Location = new Point(1058, 53);
            chatBox1.Margin = new Padding(3, 4, 3, 4);
            chatBox1.Name = "chatBox1";
            chatBox1.Size = new Size(260, 561);
            chatBox1.TabIndex = 6;
            // 
            // GameBoard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 46);
            ClientSize = new Size(1402, 690);
            Controls.Add(chatBox1);
            Controls.Add(playerCard2);
            Controls.Add(playerCard1);
            Controls.Add(panel1);
            Controls.Add(boardControl1);
            ForeColor = Color.White;
            Name = "GameBoard";
            Text = "GameBoard";
            Load += GameBoard_Load;
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private ui_components.BoardControl boardControl1;
        private Panel panel1;
        private ui_components.MenuButton menuButton3;
        private ui_components.MenuButton menuButton2;
        private ui_components.MenuButton menuButton1;
        private ui_components.MenuButton BnQuit;
        private ui_components.MenuButton BnNewGame;
        private ui_components.MenuButton btnSurrender;
        private ui_components.PlayerCard playerCard1;
        private ui_components.PlayerCard playerCard2;
        private ui_components.ChatBox chatBox1;
    }
}