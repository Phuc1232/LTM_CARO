using System;
using System.Collections.Generic;
using System.Linq;
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
        private bool _isForceClose = false;
        private bool _isGracefulExit = false;

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

            ApplyThemeColors();
        }

        private void ApplyThemeColors()
        {
            this.BackColor = UITheme.FormBackColor;
            this.ForeColor = UITheme.TextForeColor;

            if (panel1 != null)
            {
                panel1.BackColor = UITheme.CardBackColor;
            }

            if (btnSurrender != null)
            {
                btnSurrender.IsDanger = true;
                btnSurrender.ApplyThemeColors();
            }

            if (BnNewGame != null) BnNewGame.ApplyThemeColors();
            if (BnQuit != null) BnQuit.ApplyThemeColors();

            if (playerCard1 != null) playerCard1.ApplyThemeColors();
            if (playerCard2 != null) playerCard2.ApplyThemeColors();
            if (chatBox1 != null) chatBox1.ApplyThemeColors();
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

        private async void GameBoard_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // Nếu kết nối TCP đã bị ngắt từ trước (ví dụ do mất mạng đột ngột),
            // cho phép đóng Form luôn mà không hiển thị hộp thoại hoặc gửi tin đầu hàng.
            if (!TCPClientManager.Instance.IsConnected)
            {
                return;
            }

            // Nếu game đang diễn ra mà người chơi đóng cửa sổ
            if (isGameActive && !_isForceClose)
            {
                if (!_isGracefulExit)
                {
                    // Hiển thị hộp thoại xác nhận khi người dùng nhấn dấu X trên tiêu đề cửa sổ
                    DialogResult result = MessageBox.Show(
                        "Bạn có chắc chắn muốn rời trận đấu không?\nHành động này sẽ được tính là ĐẦU HÀNG và bạn sẽ quay về Trang chủ.",
                        "Xác nhận thoát trận",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        _isGracefulExit = true;

                        // Mở màn hình Home trước để tránh mất giao diện
                        var home = Application.OpenForms.OfType<Home>().FirstOrDefault() ?? new Home();
                        home.Show();
                    }
                    else
                    {
                        // Hủy việc đóng form, quay lại game
                        e.Cancel = true;
                        return;
                    }
                }

                // Xử lý gửi gói tin đầu hàng và đóng form hoàn toàn
                if (_isGracefulExit)
                {
                    e.Cancel = true; // Hủy việc đóng form tạm thời để xử lý bất đồng bộ
                    this.Hide();     // Ẩn form đi ngay lập tức

                    isGameActive = false;
                    _isForceClose = true;

                    try
                    {
                        await TCPClientManager.Instance.SendPacketAsync(
                            PacketType.SurrenderRequest,
                            new SurrenderRequestDTO { roomId = gameInfo?.roomid ?? "" });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Lỗi gửi gói tin đầu hàng: " + ex.Message);
                    }

                    this.Close(); // Gọi đóng form lần 2, lúc này _isForceClose = true nên sẽ đóng hẳn
                }
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

            // Nếu không có Form nào hiển thị (ngoài Login ẩn), hiện Home để tránh treo ngầm
            bool anyVisible = false;
            foreach (Form form in Application.OpenForms)
            {
                if (form != this && form.Visible)
                {
                    anyVisible = true;
                    break;
                }
            }
            if (!anyVisible)
            {
                var home = Application.OpenForms.OfType<Home>().FirstOrDefault() ?? new Home();
                home.Show();
            }
        }

        // =============================================
        //  NETWORK HANDLERS
        // =============================================

        private void HandleChatReceivedSafe(ChatReceiveDTO chat)
        {
            if (IsDisposed) return;
            if (InvokeRequired) { BeginInvoke(() => HandleChatReceivedSafe(chat)); return; }

            chatBox1.AddMessage(chat.fromUsername, chat.message);
        }

        private void HandleTimerUpdatedSafe(TimerUpdateDTO timer)
        {
            if (IsDisposed) return;
            if (InvokeRequired) { BeginInvoke(() => HandleTimerUpdatedSafe(timer)); return; }

            playerCard1.TimeText = $"Time: {timer.RemainingTimePlayer1}s";
            playerCard2.TimeText = $"Time: {timer.RemainingTimePlayer2}s";
            currentTurn = timer.CurrentTurnUseName;
            boardControl1.SetBoardEnabled(IsMyTurn());
        }

        private void HandleMoveNotifySafe(MoveNotifyDTO move)
        {
            if (IsDisposed) return;
            if (InvokeRequired) { BeginInvoke(() => HandleMoveNotifySafe(move)); return; }

            boardControl1.SetCell(move.row, move.col, GetSymbol(move.player));
            currentTurn = move.nextTurn;
            boardControl1.SetBoardEnabled(IsMyTurn());
        }

        private void HandleGameEndedSafe(GameEndNotifyDTO gameEnd)
        {
            // 1. Kiểm tra an toàn trước khi xử lý hoặc chuyển thread
            if (IsDisposed || !IsHandleCreated || !isGameActive) return;

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke(() => HandleGameEndedSafe(gameEnd));
                }
                catch (InvalidOperationException)
                {
                    // Form đang bị hủy/đóng đột ngột, bỏ qua
                }
                return;
            }

            // 2. Chặn re-entrancy bằng cách đặt isGameActive = false ngay lập tức trên luồng UI
            isGameActive = false;
            boardControl1.SetBoardEnabled(false);
            if (gameEnd.WinningCells != null && gameEnd.WinningCells.Count > 0)
            {
                boardControl1.HighlightWinningCells(gameEnd.WinningCells);
            }

            string message = string.IsNullOrEmpty(gameEnd.WinnerName)
                ? gameEnd.reason
                : $"{gameEnd.WinnerName} thắng!\n{gameEnd.reason}";

            // Hiển thị MessageBox an toàn
            MessageBox.Show(message, "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);

            MatchMaking matchMaking = new MatchMaking();
            matchMaking.Show();
            this.Close();
        }

        private void HandleTimerExpiredSafe(TimerExpiredDTO timer)
        {
            if (IsDisposed || !IsHandleCreated || !isGameActive) return;

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke(() => HandleTimerExpiredSafe(timer));
                }
                catch (InvalidOperationException)
                {
                }
                return;
            }

            isGameActive = false; // Gán ngay lập tức để tránh trùng lặp
            boardControl1.SetBoardEnabled(false);

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

        private async void btnSurrender_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn đầu hàng không?",
                "Đầu hàng",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (isGameActive && gameInfo != null)
                {
                    await TCPClientManager.Instance.SendPacketAsync(
                        PacketType.SurrenderRequest,
                        new SurrenderRequestDTO { roomId = gameInfo.roomid });
                }
            }
        }

        private void btnNewGame_Click(object sender, EventArgs e)
        {
            // Ván mới - tạm thời quay về Home
            DialogResult result = MessageBox.Show(
                "Bạn có muốn quay về trang chủ để chơi ván mới không?",
                "Ván mới",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _isGracefulExit = true;
                var home = Application.OpenForms.OfType<Home>().FirstOrDefault() ?? new Home();
                home.Show();
                this.Close();
            }
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            // Thoát về Home
            DialogResult result = MessageBox.Show(
                "Bạn có muốn quay về trang chủ không?",
                "Thoát",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _isGracefulExit = true;
                var home = Application.OpenForms.OfType<Home>().FirstOrDefault() ?? new Home();
                home.Show();
                this.Close();
            }
        }

        private void GameBoard_Load(object sender, EventArgs e) { }
        private void playerCard2_Load(object sender, EventArgs e) { }
        private void btnQuit_Load(object sender, EventArgs e) { }
        private void btnSurrender_Load(object sender, EventArgs e) { }
        private void btnNewGame_Load(object sender, EventArgs e) { }

        private void chatBox1_Load(object sender, EventArgs e)
        {

        }
    }
}