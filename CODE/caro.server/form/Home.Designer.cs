namespace caro.server.form
{
    partial class Home
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
            btnStart = new Button();
            btnStop = new Button();
            txtPort = new Label();
            txtPort1 = new TextBox();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.BackgroundImageLayout = ImageLayout.Zoom;
            btnStart.FlatAppearance.BorderColor = Color.SpringGreen;
            btnStart.FlatAppearance.BorderSize = 5;
            btnStart.FlatAppearance.MouseDownBackColor = Color.Red;
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.Font = new Font("Showcard Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnStart.Location = new Point(365, 149);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(94, 39);
            btnStart.TabIndex = 0;
            btnStart.Text = "Start";
            btnStart.TextAlign = ContentAlignment.TopCenter;
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += button1_Click;
            // 
            // btnStop
            // 
            btnStop.BackgroundImageLayout = ImageLayout.Zoom;
            btnStop.FlatAppearance.BorderColor = Color.IndianRed;
            btnStop.FlatAppearance.BorderSize = 5;
            btnStop.FlatAppearance.MouseDownBackColor = Color.Red;
            btnStop.FlatStyle = FlatStyle.Flat;
            btnStop.Font = new Font("Showcard Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnStop.Location = new Point(365, 240);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(94, 41);
            btnStop.TabIndex = 1;
            btnStop.Text = "Exit";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += button2_Click;
            // 
            // txtPort
            // 
            txtPort.AutoSize = true;
            txtPort.BackColor = SystemColors.ActiveCaptionText;
            txtPort.Font = new Font("Showcard Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPort.ForeColor = Color.Red;
            txtPort.Location = new Point(540, 39);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(48, 18);
            txtPort.TabIndex = 3;
            txtPort.Text = "PORT";
            txtPort.Click += label1_Click;
            // 
            // txtPort1
            // 
            txtPort1.BackColor = SystemColors.HighlightText;
            txtPort1.Cursor = Cursors.IBeam;
            txtPort1.Font = new Font("Showcard Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPort1.Location = new Point(594, 36);
            txtPort1.Name = "txtPort1";
            txtPort1.Size = new Size(125, 26);
            txtPort1.TabIndex = 5;
            txtPort1.Text = "8888";
            txtPort1.TextChanged += txtPort1_TextChanged;
            // 
            // Home
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.Choi_co_caro;
            ClientSize = new Size(784, 381);
            Controls.Add(txtPort1);
            Controls.Add(txtPort);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Name = "Home";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnStart;
        private Button btnStop;
        private Label txtPort;
        private TextBox txtPort1;
    }
}