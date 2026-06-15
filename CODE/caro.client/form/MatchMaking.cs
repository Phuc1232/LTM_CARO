using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using caro.client.network;
using caro.share.DTOs;
using caro.share.DTOs.Constants;

namespace caro.client.form
{
    public partial class MatchMaking : Form
    {
        public MatchMaking()
        {
            InitializeComponent();
            TCPClientManager.Instance.OnOnlinePlayerListUpdated += UpdateOnlinePlayers;
            TCPClientManager.Instance.OnChallengeReceived += ReceiveChallenge;
            TCPClientManager.Instance.OnChallengeResult += ChallengeResult;
            TCPClientManager.Instance.OnGameStarted += GameStarted;
            if (TCPClientManager.Instance.LastOnlinePlayers != null)
            {
                UpdateOnlinePlayers(TCPClientManager.Instance.LastOnlinePlayers);
            }
        }
        private void UpdateOnlinePlayers(OnlinePlayerListDTO dto)
        {
            if (InvokeRequired)
            {
                Invoke(() => UpdateOnlinePlayers(dto));
                return;
            }

            lstOnlinePlayers.Items.Clear();

            foreach (string player in dto.players)
            {
                // Không hiện chính mình
                if (player == TCPClientManager.Instance.CurrentUsername)
                    continue;

                lstOnlinePlayers.Items.Add(player);
            }
        }

        private async void btnChallenge_Click(object sender, EventArgs e)
        {
            if (lstOnlinePlayers.SelectedItem == null)
            {
                MessageBox.Show("Hãy chọn người chơi để thách đấu!");
                return;
            }

            string targetUsername = lstOnlinePlayers.SelectedItem.ToString();

            await TCPClientManager.Instance.SendPacketAsync(
                PacketType.ChallengeRequest,
                new ChallengeRequestDTO
                {
                    targetUsername = targetUsername
                });

            MessageBox.Show($"Đã gửi lời thách đấu tới {targetUsername}");
        }

        private async void ReceiveChallenge(ChallengeNotifyDTO dto)
        {
            if (InvokeRequired)
            {
                Invoke(() => ReceiveChallenge(dto));
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
        }

        private void ChallengeResult(ChallengeResultDTO dto)
        {
            if (InvokeRequired)
            {
                Invoke(() => ChallengeResult(dto));
                return;
            }

            MessageBox.Show(dto.message);

            if (dto.isAccepted)
            {
                GameBoard gameBoard = new GameBoard();
                gameBoard.Show();
                this.Hide();
            }
        }
        private void GameStarted(GameStartNotifyDTO dto)
        {
            if (InvokeRequired)
            {
                Invoke(() => GameStarted(dto));
                return;
            }

            GameBoard gameBoard = new GameBoard();
            gameBoard.Show();

            this.Hide();
        }
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MatchMaking_Load(object sender, EventArgs e)
        {

        }

        private async void btnChallenge_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Đã bấm nút thách đấu");

            if (lstOnlinePlayers.SelectedItem == null)
            {
                MessageBox.Show("Hãy chọn người chơi để thách đấu!");
                return;
            }

            string targetUsername = lstOnlinePlayers.SelectedItem.ToString();

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
