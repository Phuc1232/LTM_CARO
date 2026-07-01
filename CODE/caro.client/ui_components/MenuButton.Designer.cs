namespace caro.client.ui_components
{
    partial class MenuButton
    {
        
        private System.ComponentModel.IContainer components = null;

      
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code


        private void InitializeComponent()
        {
            btnMenu = new Button();
            SuspendLayout();
            // 
            // btnMenu
            // 
            btnMenu.BackColor = Color.FromArgb(0, 180, 180);
            btnMenu.BackgroundImageLayout = ImageLayout.Center;
            btnMenu.Dock = DockStyle.Fill;
            btnMenu.FlatAppearance.BorderSize = 0;
            btnMenu.FlatStyle = FlatStyle.Flat;
            btnMenu.Font = new Font("Segoe UI", 14F);
            btnMenu.ForeColor = SystemColors.ButtonHighlight;
            btnMenu.Location = new Point(0, 0);
            btnMenu.Name = "btnMenu";
            btnMenu.Size = new Size(250, 60);
            btnMenu.TabIndex = 0;
            btnMenu.Text = "Menu";
            btnMenu.UseVisualStyleBackColor = false;
            btnMenu.Click += btnMenu_Click;
            // 
            // MenuButton
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
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
