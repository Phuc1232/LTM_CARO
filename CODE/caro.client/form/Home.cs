using System.Drawing;
namespace caro.client
{
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
            this.Text = "Caro Game";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 46);
        }

        private void Home_Load(object sender, EventArgs e)
        {

        }
    }
}
