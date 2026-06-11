namespace caro.client.form
{
    partial class Login
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
            Label2 = new Label();
            Label3 = new Label();
            txtUsername = new TextBox();
            txtIpAddress = new TextBox();
            btnLogin = new caro.client.ui_components.MenuButton();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.White;
            label1.Font = new Font("Segoe UI", 24F);
            label1.Location = new Point(642, 84);
            label1.Name = "label1";
            label1.Size = new Size(104, 45);
            label1.TabIndex = 0;
            label1.Text = "CARO";
            // 
            // Label2
            // 
            Label2.AutoSize = true;
            Label2.BackColor = Color.White;
            Label2.Font = new Font("Segoe UI", 16F);
            Label2.Location = new Point(563, 149);
            Label2.Name = "Label2";
            Label2.Size = new Size(111, 30);
            Label2.TabIndex = 1;
            Label2.Text = "Username";
            // 
            // Label3
            // 
            Label3.AutoSize = true;
            Label3.BackColor = Color.White;
            Label3.Font = new Font("Segoe UI", 16F);
            Label3.Location = new Point(545, 202);
            Label3.Name = "Label3";
            Label3.Size = new Size(129, 30);
            Label3.TabIndex = 2;
            Label3.Text = "IP ADDRESS";
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(695, 156);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(182, 23);
            txtUsername.TabIndex = 3;
            // 
            // txtIpAddress
            // 
            txtIpAddress.Location = new Point(695, 211);
            txtIpAddress.Name = "txtIpAddress";
            txtIpAddress.Size = new Size(182, 23);
            txtIpAddress.TabIndex = 4;
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.White;
            btnLogin.ForeColor = Color.White;
            btnLogin.HoverBackColor = Color.DeepSkyBlue;
            btnLogin.HoverForeColor = Color.White;
            btnLogin.Location = new Point(594, 264);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(206, 37);
            btnLogin.TabIndex = 5;
            btnLogin.Text = "LOGIN";
            btnLogin.Click += btnLogin_Click;
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 46);
            ClientSize = new Size(1402, 690);
            Controls.Add(btnLogin);
            Controls.Add(txtIpAddress);
            Controls.Add(txtUsername);
            Controls.Add(Label3);
            Controls.Add(Label2);
            Controls.Add(label1);
            Name = "Login";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login";
            Load += Login_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label Label2;
        private Label Label3;
        private TextBox txtUsername;
        private TextBox txtIpAddress;
        private ui_components.MenuButton btnLogin;
    }
}