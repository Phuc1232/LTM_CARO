namespace caro.client.ui_components
{
    partial class ChatBox
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
            lblTitle = new Label();
            rtbMessages = new RichTextBox();
            txtMessage = new TextBox();
            btnSend = new Button();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 14F);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(18, 23);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(63, 32);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Chat";
            // 
            // rtbMessages
            // 
            rtbMessages.BackColor = Color.FromArgb(30, 30, 46);
            rtbMessages.BorderStyle = BorderStyle.None;
            rtbMessages.ForeColor = Color.White;
            rtbMessages.Location = new Point(18, 73);
            rtbMessages.Margin = new Padding(3, 4, 3, 4);
            rtbMessages.Name = "rtbMessages";
            rtbMessages.ReadOnly = true;
            rtbMessages.Size = new Size(263, 600);
            rtbMessages.TabIndex = 1;
            rtbMessages.Text = "";
            // 
            // txtMessage
            // 
            txtMessage.BackColor = Color.FromArgb(30, 30, 46);
            txtMessage.BorderStyle = BorderStyle.FixedSingle;
            txtMessage.Font = new Font("Segoe UI", 9F);
            txtMessage.ForeColor = Color.White;
            txtMessage.Location = new Point(17, 693);
            txtMessage.Margin = new Padding(3, 4, 3, 4);
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(183, 27);
            txtMessage.TabIndex = 2;
            txtMessage.Text = "Nhập Chat Ở Đây Lèeee";
            // 
            // btnSend
            // 
            btnSend.BackColor = Color.FromArgb(88, 101, 242);
            btnSend.FlatStyle = FlatStyle.Flat;
            btnSend.ForeColor = Color.White;
            btnSend.Location = new Point(207, 693);
            btnSend.Margin = new Padding(3, 4, 3, 4);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(74, 31);
            btnSend.TabIndex = 3;
            btnSend.Text = "➤";
            btnSend.UseVisualStyleBackColor = false;
            // 
            // ChatBox
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(42, 42, 60);
            Controls.Add(btnSend);
            Controls.Add(txtMessage);
            Controls.Add(rtbMessages);
            Controls.Add(lblTitle);
            Margin = new Padding(3, 4, 3, 4);
            Name = "ChatBox";
            Size = new Size(297, 748);
            Load += ChatBox_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private RichTextBox rtbMessages;
        private TextBox txtMessage;
        private Button btnSend;
    }
}
