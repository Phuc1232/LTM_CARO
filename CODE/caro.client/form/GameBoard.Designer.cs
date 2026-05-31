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
            playerCard1 = new caro.client.ui_components.PlayerCard();
            playerCard2 = new caro.client.ui_components.PlayerCard();
            boardControl1 = new caro.client.ui_components.BoardControl();
            SuspendLayout();
            // 
            // playerCard1
            // 
            playerCard1.Avatar = null;
            playerCard1.BackColor = Color.FromArgb(255, 255, 192);
            playerCard1.Location = new Point(649, 29);
            playerCard1.Name = "playerCard1";
            playerCard1.PlayerName = "Player";
            playerCard1.Size = new Size(156, 79);
            playerCard1.TabIndex = 0;
            playerCard1.TimeText = "Time: 20s";
            // 
            // playerCard2
            // 
            playerCard2.Avatar = null;
            playerCard2.BackColor = Color.FromArgb(255, 255, 192);
            playerCard2.Location = new Point(259, 29);
            playerCard2.Name = "playerCard2";
            playerCard2.PlayerName = "Player";
            playerCard2.Size = new Size(156, 79);
            playerCard2.TabIndex = 1;
            playerCard2.TimeText = "Time: 20s";
            playerCard2.Load += playerCard2_Load;
            // 
            // boardControl1
            // 
            boardControl1.BorderStyle = BorderStyle.FixedSingle;
            boardControl1.Location = new Point(231, 135);
            boardControl1.Name = "boardControl1";
            boardControl1.Size = new Size(598, 598);
            boardControl1.TabIndex = 2;
            // 
            // GameBoard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 255, 192);
            ClientSize = new Size(1184, 761);
            Controls.Add(boardControl1);
            Controls.Add(playerCard2);
            Controls.Add(playerCard1);
            Name = "GameBoard";
            Text = "GameBoard";
            ResumeLayout(false);
        }

        #endregion

        private ui_components.PlayerCard playerCard1;
        private ui_components.PlayerCard playerCard2;
        private ui_components.BoardControl boardControl1;
    }
}