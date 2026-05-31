using System.Net.NetworkInformation;

namespace caro.client.ui_components
{
    partial class MenuButton
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
        
        private void InitializeComponet()
        {
            btnMenu = new Button();
            SuspendLayout();

            btnMenu.BackColor = Color.FromArgb(255, 128, 255);
            btnMenu.BackgroundImageLayout = ImageLayout.Center;
            btnMenu.Dock = DockStyle.Fill;
            btnMenu.FlatAppearance.BorderSize = 0;
            btnMenu.FlatStyle = FlatStyle.Flat;
            btnMenu.Font = new Font("Seogoe UI", 14F);
            btnMenu.Location = new Point(0, 0);
            btnMenu.Name = "btnMenu";
            btnMenu.Size = new Size(250, 60);
            btnMenu.TabIndex = 0;
            btnMenu.Text = "Menu";
            btnMenu.UseVisualStyleBackColor = false;
            btnMenu.Click += BtnMenu_Click;

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnMenu);
            Name = "MenuButton";
            Size = new Size(250, 60);
            ResumeLayout(false);
        }

        private void BtnMenu_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnMenu = new Button();
            SuspendLayout();
            // 
            // btnMenu
            // 
            btnMenu.BackColor = Color.FromArgb(255, 192, 192);
            btnMenu.Dock = DockStyle.Fill;
            btnMenu.FlatAppearance.BorderSize = 0;
            btnMenu.FlatStyle = FlatStyle.Flat;
            btnMenu.Font = new Font("Segoe UI", 14F);
            btnMenu.Location = new Point(0, 0);
            btnMenu.Name = "btnMenu";
            btnMenu.Size = new Size(250, 60);
            btnMenu.TabIndex = 0;
            btnMenu.Text = "Menu";
            btnMenu.UseVisualStyleBackColor = false;
            // 
            // MenuButton
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnMenu);
            Name = "MenuButton";
            Size = new Size(250, 60);
            ResumeLayout(false);
        }

        #endregion

        private Button btnMenu;
    }
}
