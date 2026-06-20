using System;
using System.Collections.Generic;
using System.Windows.Forms;
using caro.client.network;
using caro.share.DTOs;
using caro.share.DTOs.Constants;

namespace caro.client.form
{
    public partial class GameBoard : Form
    {
        private readonly GameStartNotifyDTO? gameInfo;
        private readonly Dictionary<string, string> playerSymbols = new Dictionary<string, string>();
        private string currentTurn = "";
        private bool isGameActive = true;

        public GameBoard() : this(null)
        {
        }

        public GameBoard(GameStartNotifyDTO? dto)
        {
            InitializeComponent();

            // Đăng ký sự kiện UI
            boardControl1.OnCellClicked += BoardControl_CellClicked;
            FormClosed += GameBoard_FormClosed;
            FormClosing += GameBoard_FormClosing;

            // Đăng ký sự kiện mạng
            TCPClientManager.Instance.OnChatReceived += HandleChatReceivedSafe;
            RegisterNetworkEvents();

            // Đăng ký gửi chat
            chatBox1.OnSendMessage += async message =>
            {
                chatBox1.AddMessage("Bạn", message);
                await TCPClientManager.Instance.SendPacketAsync(
                    PacketType.ChatSend,
                    new ChatSendDTO { message = message });
            };

            // Setup thông tin người chơi
            gameInfo = dto;
            SetupPlayers(dto);
        }

        // =============================================
        //  SETUP
        // =============================================

        private void SetupPlayers(GameStartNotifyDTO? dto)
        {
            if (dto == null)
            {
                boardControl1.SetBoardEnabled(true);
                return;
            }

            string me = TCPClientManager.Instance.CurrentUsername;
            string p1 = dto.name_player1;
            string p2 = dto.name_player2;

            playerSymbols[p1] = "X";
            playerSymbols[p2] = "O";
            currentTurn = p1;

            playerCard1.PlayerName = p1 == me ? $"{p1} (Bạn) - X" : $"{p1} - X";
            playerCard2.PlayerName = p2 == me ? $"{p2} (Bạn) - O" : $"{p2} - O";

            playerCard1.TimeText = $"Time: {dto.timeSeconds}s";
            playerCard2.TimeText = $"Time: {dto.timeSeconds}s";

            boardControl1.SetBoardEnabled(IsMyTurn());
        }

        private void RegisterNetworkEvents()
        {
            TCPClientManager.Instance.OnMoveNotify += HandleMoveNotifySafe;
            TCPClientManager.Instance.OnGameEnded += HandleGameEndedSafe;
            TCPClientManager.Instance.OnTimerUpdated += HandleTimerUpdatedSafe;
            TCPClientManager.Instance.OnTimerExpired += HandleTimerExpiredSafe;
        }

        // =============================================
        //  FORM EVENTS
        // =============================================

        private void GameBoard_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // Nếu game đang diễn ra mà người chơi đóng cửa sổ → cắt kết nối
            if (isGameActive)
            {
                isGameActive = false;
                TCPClientManager.Instance.Disconnect();
            }
        }

        private void GameBoard_FormClosed(object? sender, FormClosedEventArgs e)
        {
            // Hủy đăng ký tất cả events khi form đóng để tránh memory leak
            TCPClientManager.Instance.OnMoveNotify -= HandleMoveNotifySafe;
            TCPClientManager.Instance.OnGameEnded -= HandleGameEndedSafe;
            TCPClientManager.Instance.OnTimerUpdated -= HandleTimerUpdatedSafe;
            TCPClientManager.Instance.OnTimerExpired -= HandleTimerExpiredSafe;
            TCPClientManager.Instance.OnChatReceived -= HandleChatReceivedSafe;
        }

        // =============================================
        //  NETWORK HANDLERS
        // =============================================

        private void HandleChatReceivedSafe(ChatReceiveDTO chat)
        {
            if (IsDisposed) return;
            if (InvokeRequired) { Invoke(() => HandleChatReceivedSafe(chat)); return; }

            chatBox1.AddMessage(chat.fromUsername, chat.message);
        }

        private void HandleMoveNotifySafe(MoveNotifyDTO move)
        {
            if (IsDisposed) return;
            if (InvokeRequired) { Invoke(() => HandleMoveNotifySafe(move)); return; }

            boardControl1.SetCell(move.row, move.col, GetSymbol(move.player));
            currentTurn = move.nextTurn;
            boardControl1.SetBoardEnabled(IsMyTurn());
        }

        private void HandleGameEndedSafe(GameEndNotifyDTO gameEnd)
        {
            if (IsDisposed) return;
            if (InvokeRequired) { Invoke(() => HandleGameEndedSafe(gameEnd)); return; }

            boardControl1.SetBoardEnabled(false);
            isGameActive = false;

            string message = string.IsNullOrEmpty(gameEnd.WinnerName)
                ? gameEnd.reason
                : $"{gameEnd.WinnerName} thắng!\n{gameEnd.reason}";

            MessageBox.Show(message, "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);

            MatchMaking matchMaking = new MatchMaking();
            matchMaking.Show();
            this.Close();
        }

        private void HandleTimerUpdatedSafe(TimerUpdateDTO timer)
        {
            if (IsDisposed) return;
            if (InvokeRequired) { Invoke(() => HandleTimerUpdatedSafe(timer)); return; }

            playerCard1.TimeText = $"Time: {timer.RemainingTimePlayer1}s";
            playerCard2.TimeText = $"Time: {timer.RemainingTimePlayer2}s";
            currentTurn = timer.CurrentTurnUseName;
            boardControl1.SetBoardEnabled(IsMyTurn());
        }

        private void HandleTimerExpiredSafe(TimerExpiredDTO timer)
        {
            if (IsDisposed) return;
            if (InvokeRequired) { Invoke(() => HandleTimerExpiredSafe(timer)); return; }

            boardControl1.SetBoardEnabled(false);
            isGameActive = false;
            MessageBox.Show(timer.message, "Hết giờ", MessageBoxButtons.OK, MessageBoxIcon.Information);

            MatchMaking matchMaking = new MatchMaking();
            matchMaking.Show();
            this.Close();
        }

        // =============================================
        //  BOARD CLICK - Người chơi đặt quân
        // =============================================

        private async void BoardControl_CellClicked(int row, int col)
        {
            if (!IsMyTurn())
            {
                MessageBox.Show("Chưa tới lượt bạn!");
                return;
            }

            boardControl1.SetBoardEnabled(false);

            await TCPClientManager.Instance.SendPacketAsync(
                PacketType.MoveRequest,
                new MoveRequestDTO { row = row, col = col });
        }

        // =============================================
        //  HELPERS
        // =============================================

        private bool IsMyTurn()
        {
            if (string.IsNullOrEmpty(currentTurn)) return true;
            return currentTurn == TCPClientManager.Instance.CurrentUsername;
        }

        private string GetSymbol(string player)
        {
            if (!playerSymbols.ContainsKey(player))
                playerSymbols[player] = playerSymbols.Count == 0 ? "X" : "O";
            return playerSymbols[player];
        }

        // =============================================
        //  BUTTON HANDLERS
        // =============================================

        private void menuButton4_Click(object sender, EventArgs e)
        {
            // Đầu hàng - chưa có packet phía server
            MessageBox.Show("Chức năng đầu hàng chưa có packet bên server, tạm thời chưa dùng nha.");
        }

        private void menuButton5_Click(object sender, EventArgs e)
        {
            // Ván mới - tạm thời quay về Home
            DialogResult result = MessageBox.Show(
                "Bạn có muốn quay về trang chủ để chơi ván mới không?",
                "Ván mới",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Home home = new Home();
                home.Show();
                this.Close();
            }
        }

        private void menuButton6_Click(object sender, EventArgs e)
        {
            // Thoát về Home
            DialogResult result = MessageBox.Show(
                "Bạn có muốn quay về trang chủ không?",
                "Thoát",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Home home = new Home();
                home.Show();
                this.Close();
            }
        }

        private void GameBoard_Load(object sender, EventArgs e) { }
        private void playerCard2_Load(object sender, EventArgs e) { }
        private void menuButton6_Load(object sender, EventArgs e) { }
        private void menuButton4_Load(object sender, EventArgs e) { }
        private void btnSurrender_Click(object sender, EventArgs e) { }
    }
}