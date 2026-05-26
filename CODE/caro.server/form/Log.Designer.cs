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
            lblStatus = new Label();
            button1 = new Button();
            SuspendLayout();
            // 
            // lstPlayers
            // 
            lstPlayers.FormattingEnabled = true;
            lstPlayers.Location = new Point(12, 12);
            lstPlayers.Name = "lstPlayers";
            lstPlayers.Size = new Size(363, 244);
            lstPlayers.TabIndex = 0;
            lstPlayers.SelectedIndexChanged += lstPlayers_SelectedIndexChanged;
            // 
            // rtbLogs
            // 
            rtbLogs.Location = new Point(424, 12);
            rtbLogs.Name = "rtbLogs";
            rtbLogs.ReadOnly = true;
            rtbLogs.Size = new Size(194, 244);
            rtbLogs.TabIndex = 1;
            rtbLogs.Text = "";
            rtbLogs.TextChanged += txtlogs_TextChanged;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(650, 129);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(120, 20);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "server đang chạy";
            lblStatus.Click += label1_Click;
            // 
            // button1
            // 
            button1.Location = new Point(311, 305);
            button1.Name = "button1";
            button1.Size = new Size(159, 81);
            button1.TabIndex = 3;
            button1.Text = "Stop";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button1);
            Controls.Add(lblStatus);
            Controls.Add(rtbLogs);
            Controls.Add(lstPlayers);
            Name = "Form2";
            Text = "Form2";
            Load += Form2_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox lstPlayers;
        private RichTextBox rtbLogs;
        private Label lblStatus;
        private Button button1;
    }
}