using caro.client.form;
using caro.client.network;
using System;
using System.Drawing;
using System.Windows.Forms;

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

            // ĐĂNG KÝ SỰ KIỆN CLICK CHO CÁC NÚT TẠI ĐÂY:
            menuButton2.Click += menuButton2_Click; // Nút Play With AI
            menuButton3.Click += menuButton3_Click; // Nút Match History
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

        // HÀM XỬ LÝ CLICK NÚT CHƠI VỚI AI:
        private void menuButton2_Click(object? sender, EventArgs e)
        {
            GameBoard gameBoard = new GameBoard(dto: null, isAiMode: true);
            gameBoard.Show();
            this.Hide();
        }

        // HÀM XỬ LÝ CLICK NÚT LỊCH SỬ ĐẤU:
        private void menuButton3_Click(object? sender, EventArgs e)
        {
            MatchHistory matchHistory = new MatchHistory();
            matchHistory.Show();
            this.Hide();
        }
    }
}