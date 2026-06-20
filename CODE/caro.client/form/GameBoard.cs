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
        private bool isAiMode = false;
        private int[,] aiBoardState = new int[15, 15]; 
        private int userPlayerId = 1; 
        private int aiPlayerId = 2;  

        public GameBoard() : this(null)
        {
        }
        public GameBoard(GameStartNotifyDTO? dto, bool isAiMode = false)
        {
            InitializeComponent();
            this.isAiMode = isAiMode; 
            chatBox1.OnSendMessage += async message =>
            {
                chatBox1.AddMessage("Bạn", message);
                if (!isAiMode)
                {
                    await TCPClientManager.Instance.SendPacketAsync(
                        PacketType.ChatSend,
                        new ChatSendDTO
                        {
                            message = message
                        });
                }
                else
                {
                    
                    chatBox1.AddMessage("AI Máy", "Tôi đang tập trung đánh cờ, đừng làm phiền tôi nhé!");
                }
            };
            if (!isAiMode)
            {
                TCPClientManager.Instance.OnChatReceived += HandleChatReceivedSafe;
                gameInfo = dto;
                SetupPlayers(dto);
                RegisterNetworkEvents();
            }
            else
            {
                
                playerCard1.PlayerName = "Bạn (X)";
                playerCard2.PlayerName = "AI Máy (O)";
                playerCard1.TimeText = "Không giới hạn";
                playerCard2.TimeText = "Không giới hạn";
                boardControl1.SetBoardEnabled(true);
            }
            boardControl1.OnCellClicked += BoardControl_CellClicked;
            FormClosed += GameBoard_FormClosed;
            FormClosing += GameBoard_FormClosing;
        }

      
        private void GameBoard_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (isGameActive)
            {
                isGameActive = false; 
                TCPClientManager.Instance.Disconnect();
            }
        }
        private void HandleChatReceivedSafe(ChatReceiveDTO chat)
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(() => HandleChatReceivedSafe(chat));
                return;
            }

            chatBox1.AddMessage(chat.fromUsername, chat.message);
        }

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

        private void GameBoard_FormClosed(object? sender, FormClosedEventArgs e)
        {
            TCPClientManager.Instance.OnMoveNotify -= HandleMoveNotifySafe;
            TCPClientManager.Instance.OnGameEnded -= HandleGameEndedSafe;
            TCPClientManager.Instance.OnTimerUpdated -= HandleTimerUpdatedSafe;
            TCPClientManager.Instance.OnTimerExpired -= HandleTimerExpiredSafe;
            TCPClientManager.Instance.OnChatReceived -= HandleChatReceivedSafe;
        }

        private async void BoardControl_CellClicked(int row, int col)
        {
            if (isAiMode)
            {
                
                boardControl1.SetCell(row, col, "X");
                aiBoardState[row, col] = userPlayerId;
                boardControl1.SetBoardEnabled(false); 
                                                      
                var status = caro.ai.AIServices.CheckWin(aiBoardState);
                if (status.Winner != null || status.IsDraw)
                {
                    string msg = status.Winner == userPlayerId ? "Chúc mừng! Bạn đã thắng AI!" : "Trận đấu hòa!";
                    MessageBox.Show(msg, "Trò chơi kết thúc", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Home home = new Home();
                    home.Show();
                    this.Close();
                    return;
                }
                
                
                var result = await Task.Run(() => caro.ai.AIServices.MiniMax(aiBoardState, 3, int.MinValue, int.MaxValue, true, aiPlayerId));

                int aiRow = 7, aiCol = 7;
                if (result.move.HasValue)
                {
                    aiRow = result.move.Value.r;
                    aiCol = result.move.Value.c;
                }
                else
                {
                    
                    for (int r = 0; r < 15; r++)
                    {
                        for (int c = 0; c < 15; c++)
                        {
                            if (aiBoardState[r, c] == 0) { aiRow = r; aiCol = c; break; }
                        }
                    }
                }
               
                boardControl1.SetCell(aiRow, aiCol, "O");
                aiBoardState[aiRow, aiCol] = aiPlayerId;
               
                status = caro.ai.AIServices.CheckWin(aiBoardState);
                if (status.Winner != null || status.IsDraw)
                {
                    string msg = status.Winner == aiPlayerId ? "AI Máy đã thắng cuộc! Thử lại nhé." : "Trận đấu hòa!";
                    MessageBox.Show(msg, "Trò chơi kết thúc", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Home home = new Home();
                    home.Show();
                    this.Close();
                    return;
                }
                
                boardControl1.SetBoardEnabled(true);
                return;
            }
            // Logic chơi Online cũ (giữ nguyên):
            if (!IsMyTurn())
            {
                MessageBox.Show("Chưa tới lượt bạn!");
                return;
            }
            boardControl1.SetBoardEnabled(false);
            await TCPClientManager.Instance.SendPacketAsync(
                PacketType.MoveRequest,
                new MoveRequestDTO
                {
                    row = row,
                    col = col
                });
        }

        private bool IsMyTurn()
        {
            if (string.IsNullOrEmpty(currentTurn)) return true;
            return currentTurn == TCPClientManager.Instance.CurrentUsername;
        }

        private string GetSymbol(string player)
        {
            if (!playerSymbols.ContainsKey(player))
            {
                playerSymbols[player] = playerSymbols.Count == 0 ? "X" : "O";
            }

            return playerSymbols[player];
        }

        private void HandleMoveNotifySafe(MoveNotifyDTO move)
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(() => HandleMoveNotifySafe(move));
                return;
            }

            string symbol = GetSymbol(move.player);
            boardControl1.SetCell(move.row, move.col, symbol);

            currentTurn = move.nextTurn;
            boardControl1.SetBoardEnabled(IsMyTurn());
        }

        private void HandleGameEndedSafe(GameEndNotifyDTO gameEnd)
        {
            if (IsDisposed) return;
            if (InvokeRequired)
            {
                Invoke(() => HandleGameEndedSafe(gameEnd));
                return;
            }
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

            if (InvokeRequired)
            {
                Invoke(() => HandleTimerUpdatedSafe(timer));
                return;
            }

            playerCard1.TimeText = $"Time: {timer.RemainingTimePlayer1}s";
            playerCard2.TimeText = $"Time: {timer.RemainingTimePlayer2}s";

            currentTurn = timer.CurrentTurnUseName;
            boardControl1.SetBoardEnabled(IsMyTurn());
        }

        private void HandleTimerExpiredSafe(TimerExpiredDTO timer)
        {
            if (IsDisposed) return;
            if (InvokeRequired)
            {
                Invoke(() => HandleTimerExpiredSafe(timer));
                return;
            }
            boardControl1.SetBoardEnabled(false);
            isGameActive = false; 
            MessageBox.Show(timer.message, "Hết giờ", MessageBoxButtons.OK, MessageBoxIcon.Information);
           
            MatchMaking matchMaking = new MatchMaking();
            matchMaking.Show();
            this.Close();
        }

        private void playerCard2_Load(object sender, EventArgs e)
        {
        }

        private void menuButton6_Load(object sender, EventArgs e)
        {
        }

        private void menuButton4_Load(object sender, EventArgs e)
        {
        }

        private void btnSurrender_Click(object sender, EventArgs e)
        {
        }

        private void menuButton4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng đầu hàng chưa có packet bên server, tạm thời chưa dùng nha.");
        }

        private void menuButton6_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có muốn quay về trang chủ không?",
                "Quit",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Home home = new Home();
                home.Show();
                this.Close();
            }
        }

        private void GameBoard_Load(object sender, EventArgs e)
        {
        }

        private void menuButton5_Click(object sender, EventArgs e)
        {
            if (isAiMode)
            {
                Home home = new Home();
                home.Show();
                this.Close();
                return;
            }

          
        }
       

    }
}