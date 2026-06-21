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
        private bool _isSubscribed = false;

        public Home()
        {
            InitializeComponent();
            this.Text = "Caro Game";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 46);

            // Đăng ký sự kiện click cho các nút
            btnPlayWithAI.Click += btnPlayWithAI_Click; // Nút Play With AI
            btnMatchHistory.Click += btnMatchHistory_Click; // Nút Match History
            btnLogOut.Click += (s, e) => this.Close(); // Nút Log Out

            this.FormClosing += Home_FormClosing;
            this.VisibleChanged += Home_VisibleChanged;

            if (this.Visible)
            {
                SubscribeEvents();
            }
        }

        private void Home_FormClosing(object? sender, FormClosingEventArgs e)
        {
            UnsubscribeEvents();

            // Hiển thị lại Form đăng nhập (ẩn ở chế độ background)
            var loginForm = Application.OpenForms.OfType<Login>().FirstOrDefault();
            if (loginForm != null)
            {
                loginForm.Show();
            }

            // Ngắt kết nối socket tới server (báo offline và cập nhật danh sách người online)
            TCPClientManager.Instance.Disconnect();
        }

        private void SubscribeEvents()
        {
            if (!_isSubscribed)
            {
                TCPClientManager.Instance.OnChallengeResult += HandleChallengeResultSafe;
                TCPClientManager.Instance.OnGameStarted += HandleGameStartedSafe;
                _isSubscribed = true;
            }
        }

        private void UnsubscribeEvents()
        {
            if (_isSubscribed)
            {
                TCPClientManager.Instance.OnChallengeResult -= HandleChallengeResultSafe;
                TCPClientManager.Instance.OnGameStarted -= HandleGameStartedSafe;
                _isSubscribed = false;
            }
        }

        private void Home_VisibleChanged(object? sender, EventArgs e)
        {
            if (this.Visible)
            {
                SubscribeEvents();
            }
            else
            {
                UnsubscribeEvents();
            }
        }

        private void HandleChallengeResultSafe(ChallengeResultDTO dto)
        {
            if (IsDisposed) return;
            if (InvokeRequired)
            {
                BeginInvoke(() => HandleChallengeResultSafe(dto));
                return;
            }

            MessageBox.Show(dto.message);
        }

        private void HandleGameStartedSafe(GameStartNotifyDTO dto)
        {
            if (IsDisposed) return;
            if (InvokeRequired)
            {
                BeginInvoke(() => HandleGameStartedSafe(dto));
                return;
            }

            GameBoard gameBoard = new GameBoard(dto);
            gameBoard.Show();
            this.Hide();
        }

        private void Home_Load(object sender, EventArgs e) { }
        private void btnPlayWithAI_Load(object sender, EventArgs e) { }
        private void btnLogOut_Load(object sender, EventArgs e) { }
        private void btnPlayOnline_Load(object sender, EventArgs e) { }

        // =============================================
        //  Nút Play Online
        // =============================================
        private void btnPlayOnline_Click(object sender, EventArgs e)
        {
            MatchMaking matchMaking = new MatchMaking();
            matchMaking.Show();
            this.Hide();
        }

        // =============================================
        //  Nút Play With AI
        //  → Chuyển sang màn hình MatchMaking trước để ẩn Home và hủy đăng ký sự kiện mạng
        //  → Gửi ChallengeRequest tới AI_Bot qua Server
        // =============================================
        private async void btnPlayWithAI_Click(object? sender, EventArgs e)
        {
            // Gửi lời thách đấu tới AI_Bot. GameBoard sẽ tự động mở từ Home khi nhận được GameStartNotify.
            await TCPClientManager.Instance.SendPacketAsync(
                PacketType.ChallengeRequest,
                new ChallengeRequestDTO
                {
                    targetUsername = "AI_Bot"
                });
        }

        // =============================================
        //  Nút Match History
        // =============================================
        private void btnMatchHistory_Click(object? sender, EventArgs e)
        {
            MatchHistory matchHistory = new MatchHistory();
            matchHistory.Show();
            this.Hide();
        }
    }
}