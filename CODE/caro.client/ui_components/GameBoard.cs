using caro.client.network;
using CaroGame.Client.UI_Components;
using caro.share.DTOs;
using caro.share.DTOs.Constants;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CaroGame.Client.Forms
{
    public class GameBoard : Form
    {
        private CaroBoard _board = new CaroBoard();
        private Label lblTurn = null!;
        private Label lblTimer = null!;

        private string _player1Name;
        private string _player2Name;
        private string _myUsername;
        private bool _isGameActive = true;

        public GameBoard() : this("Player 1", "Player 2", "Player 1")
        {
        }

        public GameBoard(string player1Name, string player2Name, string myUsername)
        {
            _player1Name = player1Name;
            _player2Name = player2Name;
            _myUsername = myUsername;

            this.Text = $"Caro Match: {player1Name} vs {player2Name}";
            this.Size = new Size(490, 610);
            this.BackColor = Color.FromArgb(30, 30, 47); // Dark Theme hiện đại đồng nhất
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Thiết lập Nhãn hiển thị Lượt đi
            lblTurn = new Label
            {
                Text = $"Lượt đi: {player1Name} (Quân X) - Bạn là quân {(myUsername == player1Name ? "X" : "O")}",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(241, 196, 15), // Màu vàng nổi bật
                Location = new Point(10, 10),
                Size = new Size(455, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblTurn);

            // Thiết lập Nhãn hiển thị Thời gian đếm ngược
            lblTimer = new Label
            {
                Text = $"Đang tải thời gian...",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(46, 204, 113), // Màu xanh lá sáng
                Location = new Point(10, 35),
                Size = new Size(455, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblTimer);

            // Thiết lập vị trí bàn cờ (tịnh tiến xuống dưới các nhãn hiển thị)
            _board.Location = new Point(10, 65);
            _board.CellClicked += OnCellClicked;
            this.Controls.Add(_board);

            // Đăng ký sự kiện mạng
            TCPClientManager.Instance.OnMoveNotify += HandleMoveNotify;
            TCPClientManager.Instance.OnTimerUpdated += HandleTimerUpdated;
            TCPClientManager.Instance.OnGameEnded += HandleGameEnded;
            TCPClientManager.Instance.OnDisconnected += HandleDisconnected;
        }

        private void HandleMoveNotify(MoveNotifyDTO move)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => HandleMoveNotify(move)));
                return;
            }

            // Vẽ biểu tượng X hoặc O tương ứng
            int playerNumber = (move.player == _player1Name) ? 1 : (move.player == _player2Name) ? 2 : 0;
            if (playerNumber > 0)
            {
                _board.UpdateCell(move.row, move.col, playerNumber);
            }

            // Cập nhật lượt đi tiếp theo lên UI
            if (!string.IsNullOrEmpty(move.nextTurn))
            {
                string turnOwner = (move.nextTurn == _myUsername) ? "BẠN (Lượt của bạn)" : $"{move.nextTurn} (Chờ đối thủ)";
                lblTurn.Text = $"Lượt đi: {turnOwner} (Quân {(move.nextTurn == _player1Name ? "X" : "O")})";
                lblTurn.ForeColor = (move.nextTurn == _myUsername) ? Color.FromArgb(46, 204, 113) : Color.FromArgb(241, 196, 15);
            }
        }

        private void HandleTimerUpdated(TimerUpdateDTO timer)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => HandleTimerUpdated(timer)));
                return;
            }

            lblTimer.Text = $"Thời gian: {_player1Name} ({timer.RemainingTimePlayer1}s) | {_player2Name} ({timer.RemainingTimePlayer2}s)";

            // Highlight đỏ cảnh báo khi thời gian của lượt hiện tại sắp hết (< 30 giây)
            bool isP1Low = (timer.CurrentTurnUseName == _player1Name && timer.RemainingTimePlayer1 < 30);
            bool isP2Low = (timer.CurrentTurnUseName == _player2Name && timer.RemainingTimePlayer2 < 30);

            if (isP1Low || isP2Low)
            {
                lblTimer.ForeColor = Color.FromArgb(231, 76, 60); // Đỏ cảnh báo
            }
            else
            {
                lblTimer.ForeColor = Color.FromArgb(46, 204, 113); // Xanh lá
            }
        }

        private void HandleGameEnded(GameEndNotifyDTO endInfo)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => HandleGameEnded(endInfo)));
                return;
            }

            _isGameActive = false; // Đánh dấu game kết thúc sạch

            string winnerMsg = string.IsNullOrEmpty(endInfo.WinnerName)
                ? $"Trận đấu kết thúc! Lý do: {endInfo.reason}"
                : $"Người chiến thắng: {endInfo.WinnerName}!\nLý do: {endInfo.reason}";

            MessageBox.Show(winnerMsg, "Kết thúc trận đấu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            CleanupEvents();
            this.Close();
        }

        private void HandleDisconnected()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(HandleDisconnected));
                return;
            }

            _isGameActive = false;
            CleanupEvents();
            this.Close();
        }

        private async void OnCellClicked(object? sender, Point e)
        {
            // Point e chứa X (hàng) và Y (cột) từ CaroBoard
            int row = e.X;
            int col = e.Y;

            var moveReq = new MoveRequestDTO
            {
                row = row,
                col = col
            };

            await TCPClientManager.Instance.SendPacketAsync(PacketType.MoveRequest, moveReq);
        }

        private void CleanupEvents()
        {
            TCPClientManager.Instance.OnMoveNotify -= HandleMoveNotify;
            TCPClientManager.Instance.OnTimerUpdated -= HandleTimerUpdated;
            TCPClientManager.Instance.OnGameEnded -= HandleGameEnded;
            TCPClientManager.Instance.OnDisconnected -= HandleDisconnected;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            CleanupEvents();

            // Nếu người chơi chủ động tắt cửa sổ khi trận đấu đang hoạt động (rage quit)
            // Ngắt kết nối thô để Server xử thua và dọn dẹp phòng chơi, tránh treo đối thủ
            if (_isGameActive)
            {
                _isGameActive = false;
                TCPClientManager.Instance.Disconnect();
            }

            base.OnFormClosed(e);
        }
    }
}