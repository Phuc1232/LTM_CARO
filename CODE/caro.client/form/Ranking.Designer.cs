namespace caro.client.form
{
    partial class Ranking
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // Ranking
            // 
            ClientSize = new Size(284, 261);
            Name = "Ranking";
            Load += Ranking_Load;
            ResumeLayout(false);
        }
    }
}