using System;
using System.Linq;
using System.Windows.Forms;
using caro.client.network;
using caro.share.DTOs;
using caro.share.DTOs.Constants;

namespace caro.client.form
{
    public partial class MatchMaking : Form
    {
        private bool isGameOpened = false;

        public MatchMaking()
        {
            InitializeComponent();

            TCPClientManager.Instance.OnlinePlayerListUpdated += UpdateOnlinePlayers;
            TCPClientManager.Instance.OnChallengeReceived += ReceiveChallenge;
            TCPClientManager.Instance.OnChallengeResult += ChallengeResult;
            TCPClientManager.Instance.OnGameStarted += GameStarted;

            btnBack.Click += btnBack_Click;
            FormClosed += MatchMaking_FormClosed;

            if (TCPClientManager.Instance.LastOnlinePlayers != null)
            {
                UpdateOnlinePlayers(TCPClientManager.Instance.LastOnlinePlayers);
            }

            ApplyThemeColors();
        }

        private void ApplyThemeColors()
        {
            this.BackColor = UITheme.FormBackColor;
            this.ForeColor = UITheme.TextForeColor;

            label1.BackColor = Color.Transparent;
            label1.ForeColor = UITheme.TitleColor;
            label1.Font = new Font("Segoe UI Semibold", 28F, FontStyle.Bold);

            lstOnlinePlayers.BackColor = UITheme.InputBackColor;
            lstOnlinePlayers.ForeColor = UITheme.InputForeColor;
            lstOnlinePlayers.BorderStyle = BorderStyle.FixedSingle;
            lstOnlinePlayers.Font = new Font("Segoe UI", 16F);

            if (btnChallenge != null) btnChallenge.ApplyThemeColors();
            if (btnBack != null)
            {
                btnBack.IsDanger = true;
                btnBack.ApplyThemeColors();
            }
        }

        private void MatchMaking_FormClosed(object? sender, FormClosedEventArgs e)
        {
            UnsubscribeNetworkEvents();
        }

        private void UnsubscribeNetworkEvents()
        {
            TCPClientManager.Instance.OnlinePlayerListUpdated -= UpdateOnlinePlayers;
            TCPClientManager.Instance.OnChallengeReceived -= ReceiveChallenge;
            TCPClientManager.Instance.OnChallengeResult -= ChallengeResult;
            TCPClientManager.Instance.OnGameStarted -= GameStarted;
        }

        private void UpdateOnlinePlayers(OnlinePlayerListDTO dto)
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                BeginInvoke(() => UpdateOnlinePlayers(dto));
                return;
            }

            lstOnlinePlayers.Items.Clear();

            foreach (string player in dto.players)
            {
                if (player == TCPClientManager.Instance.CurrentUsername)
                    continue;

                lstOnlinePlayers.Items.Add(player);
            }
        }

        private void OpenGameBoard(GameStartNotifyDTO dto)
        {
            if (isGameOpened) return;
            isGameOpened = true;

            GameBoard gameBoard = new GameBoard(dto);
            gameBoard.Show();

            UnsubscribeNetworkEvents();
            this.Close();
        }

        private async void ReceiveChallenge(ChallengeNotifyDTO dto)
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                BeginInvoke(() => ReceiveChallenge(dto));
                return;
            }

            DialogResult result = MessageBox.Show(
                $"{dto.fromUsername} muốn thách đấu bạn. Đồng ý không?",
                "Lời mời thách đấu",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            await TCPClientManager.Instance.SendPacketAsync(
                PacketType.ChallengeResponse,
                new ChallengeResponseDTO
                {
                    roomId = dto.roomId,
                    isAccepted = result == DialogResult.Yes
                });

            // Không mở GameBoard ở đây.
            // Đợi server gửi GameStartNotify.
        }

        private void ChallengeResult(ChallengeResultDTO dto)
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                BeginInvoke(() => ChallengeResult(dto));
                return;
            }

            MessageBox.Show(dto.message);

            // Không mở GameBoard ở đây.
            // Đợi server gửi GameStartNotify.
        }

        private void GameStarted(GameStartNotifyDTO dto)
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                BeginInvoke(() => GameStarted(dto));
                return;
            }

            OpenGameBoard(dto);
        }

        private void btnBack_Click(object? sender, EventArgs e)
        {
            var home = Application.OpenForms.OfType<Home>().FirstOrDefault() ?? new Home();
            home.Show();
            this.Close();
        }

        private void MatchMaking_Load(object sender, EventArgs e)
        {
        }

        private async void btnChallenge_Click_1(object sender, EventArgs e)
        {
            if (lstOnlinePlayers.SelectedItem == null)
            {
                MessageBox.Show("Hãy chọn người chơi để thách đấu!");
                return;
            }

            string targetUsername = lstOnlinePlayers.SelectedItem.ToString()!;

            await TCPClientManager.Instance.SendPacketAsync(
                PacketType.ChallengeRequest,
                new ChallengeRequestDTO
                {
                    targetUsername = targetUsername
                });

            MessageBox.Show($"Đã gửi lời thách đấu tới {targetUsername}");
        }
    }
}