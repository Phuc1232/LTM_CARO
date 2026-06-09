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
            bool connected = await TCPClientManager.Instance.ConnectAsync("127.0.0.1", 8888);

            if (connected)
            {
                MessageBox.Show("Kết nối server thành công!");

                GameBoard gameBoard = new GameBoard();
                gameBoard.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show(
                    "Không thể kết nối tới server. Hãy mở server trước!",
                    "Connection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
