using caro.client.form;
using caro.client.network;
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

        private void menuButton2_Load(object sender, EventArgs e)
        {

        }

        private void menuButton5_Load(object sender, EventArgs e)
        {

        }

        private void menuButton1_Load(object sender, EventArgs e)
        {

        }

        private async void menuButton1_Click(object sender, EventArgs e)
        {
            MatchMaking matchMaking = new MatchMaking();
            matchMaking.Show();
            this.Hide();
        }
    }
}
