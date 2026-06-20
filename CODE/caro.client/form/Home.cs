using caro.client.form;
using caro.client.network;
using System;
using System.Drawing;
using System.Windows.Forms;
using caro.share.DTOs;
using caro.share.DTOs.Constants;

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

            // Đăng ký sự kiện click cho các nút
            menuButton2.Click += menuButton2_Click; // Nút Play With AI
            menuButton3.Click += menuButton3_Click; // Nút Match History
        }

        private void Home_Load(object sender, EventArgs e) { }
        private void menuButton2_Load(object sender, EventArgs e) { }
        private void menuButton5_Load(object sender, EventArgs e) { }
        private void menuButton1_Load(object sender, EventArgs e) { }

        // =============================================
        //  Nút Play Online
        // =============================================
        private void menuButton1_Click(object sender, EventArgs e)
        {
            MatchMaking matchMaking = new MatchMaking();
            matchMaking.Show();
            this.Hide();
        }

        // =============================================
        //  Nút Play With AI
        //  → Gửi ChallengeRequest tới AI_Bot qua Server
        //  → Chuyển sang MatchMaking chờ Server tạo phòng
        // =============================================
        private async void menuButton2_Click(object? sender, EventArgs e)
        {
            // Kiểm tra AI_Bot có đang online không (tùy chọn - có thể bỏ)
            // Gửi lời thách đấu tới AI_Bot
            await TCPClientManager.Instance.SendPacketAsync(
                PacketType.ChallengeRequest,
                new ChallengeRequestDTO
                {
                    targetUsername = "AI_Bot"
                });

            // Chuyển sang màn hình chờ - MatchMaking sẽ xử lý
            // ChallengeResult và GameStartNotify từ Server
            MatchMaking matchMaking = new MatchMaking();
            matchMaking.Show();
            this.Hide();
        }

        // =============================================
        //  Nút Match History
        // =============================================
        private void menuButton3_Click(object? sender, EventArgs e)
        {
            MatchHistory matchHistory = new MatchHistory();
            matchHistory.Show();
            this.Hide();
        }
    }
}