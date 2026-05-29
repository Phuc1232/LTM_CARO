namespace caro.server.form
{
    partial class Log
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
            lstPlayers = new ListBox();
            rtbLogs = new RichTextBox();
            button1 = new Button();
            SuspendLayout();
            // 
            // lstPlayers
            // 
            lstPlayers.FormattingEnabled = true;
            lstPlayers.Location = new Point(12, 12);
            lstPlayers.Name = "lstPlayers";
            lstPlayers.Size = new Size(365, 264);
            lstPlayers.TabIndex = 0;
            lstPlayers.SelectedIndexChanged += lstPlayers_SelectedIndexChanged;
            // 
            // rtbLogs
            // 
            rtbLogs.Location = new Point(437, 12);
            rtbLogs.Name = "rtbLogs";
            rtbLogs.ReadOnly = true;
            rtbLogs.Size = new Size(279, 262);
            rtbLogs.TabIndex = 1;
            rtbLogs.Text = "";
            rtbLogs.TextChanged += txtlogs_TextChanged;
            // 
            // button1
            // 
            button1.Location = new Point(309, 361);
            button1.Name = "button1";
            button1.Size = new Size(131, 56);
            button1.TabIndex = 3;
            button1.Text = "Stop";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // Log
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button1);
            Controls.Add(rtbLogs);
            Controls.Add(lstPlayers);
            Name = "Log";
            Text = "Form2";
            Load += Form2_Load;
            ResumeLayout(false);
        }

        #endregion

        private ListBox lstPlayers;
        private RichTextBox rtbLogs;
        private Button button1;
    }
}