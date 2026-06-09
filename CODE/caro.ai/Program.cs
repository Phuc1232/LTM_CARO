using System;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using caro.ai;
using caro.share;
using caro.share.DTOs;
using caro.share.DTOs.Constants;

namespace caro.bot
{
    internal class Program
    {
        private static TcpClient? _client;
        private static NetworkStream? _stream;
        private static CancellationTokenSource? _cts;
        private static bool _isConnected;

        private static string _botUsername = "AI_Bot";
        private static readonly AIServices _aiEngine = new();
        private static int _ourPlayerId = 1; // 1 for Player1 (X), 2 for Player2 (O)
        private static string _player1Name = "";
        private static string _player2Name = "";
        private static string _currentRoomId = "";
        private static bool _isMyTurn = false;

        private static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("========================================");
            Console.WriteLine("        CARO AI BOT - CLIENT            ");
            Console.WriteLine("========================================");

            string ip = "127.0.0.1";
            int port = 8888;

            Console.Write($"Nhập IP Server [Mặc định: {ip}]: ");
            string? ipInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(ipInput)) ip = ipInput.Trim();

            Console.Write($"Nhập Port Server [Mặc định: {port}]: ");
            string? portInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(portInput) && int.TryParse(portInput, out int parsedPort))
            {
                port = parsedPort;
            }

            Console.Write($"Nhập tên Bot [Mặc định: {_botUsername}]: ");
            string? nameInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nameInput)) _botUsername = nameInput.Trim();

            Console.WriteLine($"\n[Hệ thống] Đang kết nối tới Server {ip}:{port}...");
            bool connected = await ConnectAsync(ip, port);
            if (!connected)
            {
                Console.WriteLine("[Lỗi] Kết nối thất bại. Nhấn phím bất kỳ để thoát.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("[Hệ thống] Kết nối thành công! Đang gửi yêu cầu đăng nhập...");
            await SendPacketAsync(PacketType.LoginRequest, new LoginRequestDTO { username = _botUsername });

            // Giữ chương trình chạy cho đến khi ngắt kết nối
            while (_isConnected)
            {
                await Task.Delay(1000);
            }

            Console.WriteLine("\n[Hệ thống] Đã ngắt kết nối khỏi Server. Nhấn phím bất kỳ để thoát.");
            Console.ReadKey();
        }

        private static async Task<bool> ConnectAsync(string ip, int port)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(ip, port);
                _stream = _client.GetStream();
                _isConnected = true;
                _cts = new CancellationTokenSource();

                // Lắng nghe dữ liệu bất đồng bộ từ Server
                _ = Task.Run(() => ReceiveLoopAsync(_cts.Token));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lỗi kết nối] {ex.Message}");
                Disconnect();
                return false;
            }
        }

        private static void Disconnect()
        {
            if (!_isConnected) return;
            _isConnected = false;
            _cts?.Cancel();
            try { _stream?.Close(); } catch { }
            try { _client?.Close(); } catch { }
            _stream = null;
            _client = null;
            Console.WriteLine("[Hệ thống] Kết nối đã bị đóng.");
        }

        private static async Task SendPacketAsync<T>(PacketType type, T dtoData)
        {
            if (!_isConnected || _stream == null) return;
            try
            {
                var packet = new BasePacket
                {
                    Type = type,
                    payload = JsonSerializer.Serialize(dtoData)
                };
                await PacketHelper.SendPacketAsync(_stream, packet);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lỗi gửi dữ liệu] {ex.Message}");
                Disconnect();
            }
        }

        private static async Task ReceiveLoopAsync(CancellationToken token)
        {
            try
            {
                while (_isConnected && !token.IsCancellationRequested && _stream != null)
                {
                    var packet = await PacketHelper.ReceivePacketAsync<BasePacket>(_stream);
                    if (packet != null)
                    {
                        await ProcessIncomingPacketAsync(packet);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_isConnected)
                {
                    Console.WriteLine($"[Lỗi nhận dữ liệu] {ex.Message}");
                    Disconnect();
                }
            }
        }

        private static async Task ProcessIncomingPacketAsync(BasePacket packet)
        {
            switch (packet.Type)
            {
                case PacketType.LoginResponse:
                    var loginRes = JsonSerializer.Deserialize<LoginResponseDTO>(packet.payload);
                    if (loginRes != null)
                    {
                        if (loginRes.isSuccess)
                        {
                            Console.WriteLine($"[Đăng nhập] Đăng nhập thành công với tên: {_botUsername}!");
                            Console.WriteLine("[Hệ thống] Đang ở sảnh chờ. Sẵn sàng nhận lời thách đấu.");
                        }
                        else
                        {
                            Console.WriteLine($"[Đăng nhập] Thất bại: {loginRes.message}");
                            Disconnect();
                        }
                    }
                    break;

                case PacketType.ChallengeNotify:
                    var challenge = JsonSerializer.Deserialize<ChallengeNotifyDTO>(packet.payload);
                    if (challenge != null)
                    {
                        Console.WriteLine($"[Thách đấu] Nhận lời thách đấu từ '{challenge.fromUsername}' (Mã phòng: {challenge.roomId})");
                        Console.WriteLine("[Thách đấu] Tự động chấp nhận lời thách đấu...");
                        var reply = new ChallengeResponseDTO
                        {
                            roomId = challenge.roomId,
                            isAccepted = true
                        };
                        await SendPacketAsync(PacketType.ChallengeResponse, reply);
                    }
                    break;

                case PacketType.ChallengeResult:
                    var challengeResult = JsonSerializer.Deserialize<ChallengeResultDTO>(packet.payload);
                    if (challengeResult != null)
                    {
                        Console.WriteLine($"[Thách đấu] Kết quả: {(challengeResult.isAccepted ? "Chấp nhận" : "Từ chối")} - {challengeResult.message}");
                    }
                    break;

                case PacketType.GameStartNotify:
                    var gameStart = JsonSerializer.Deserialize<GameStartNotifyDTO>(packet.payload);
                    if (gameStart != null)
                    {
                        _currentRoomId = gameStart.roomid;
                        _player1Name = gameStart.name_player1;
                        _player2Name = gameStart.name_player2;
                        _aiEngine.ResetBoard();

                        Console.WriteLine("\n========================================");
                        Console.WriteLine($"[Trận đấu] BẮT ĐẦU TRẬN ĐẤU (Phòng: {_currentRoomId})");
                        Console.WriteLine($"[Trận đấu] Player 1 (X): {_player1Name}");
                        Console.WriteLine($"[Trận đấu] Player 2 (O): {_player2Name}");
                        Console.WriteLine("========================================");

                        if (_botUsername == _player1Name)
                        {
                            _ourPlayerId = 1;
                            Console.WriteLine("[Trận đấu] Bot là Quân X (Đi trước)");
                            _isMyTurn = true;
                            await MakeMoveAsync();
                        }
                        else
                        {
                            _ourPlayerId = 2;
                            Console.WriteLine("[Trận đấu] Bot là Quân O (Đi sau)");
                            _isMyTurn = false;
                        }
                    }
                    break;

                case PacketType.MoveNotiFy:
                case PacketType.MoveRequest:
                    var moveNotify = JsonSerializer.Deserialize<MoveNotifyDTO>(packet.payload);
                    if (moveNotify != null)
                    {
                        int playedVal = (moveNotify.player == _player1Name) ? 1 : 2;
                        _aiEngine.UpdateBoard(moveNotify.row, moveNotify.col, playedVal);
                        Console.WriteLine($"[Trận đấu] '{moveNotify.player}' đã đánh tại [{moveNotify.row}, {moveNotify.col}]");

                        if (moveNotify.nextTurn == _botUsername)
                        {
                            _isMyTurn = true;
                            await MakeMoveAsync();
                        }
                        else
                        {
                            _isMyTurn = false;
                        }
                    }
                    break;

                case PacketType.GameEndNotify:
                    var gameEnd = JsonSerializer.Deserialize<GameEndNotifyDTO>(packet.payload);
                    if (gameEnd != null)
                    {
                        Console.WriteLine("\n========================================");
                        Console.WriteLine($"[Kết quả] Trận đấu kết thúc!");
                        if (string.IsNullOrEmpty(gameEnd.WinnerName))
                        {
                            Console.WriteLine($"[Kết quả] Lý do: {gameEnd.reason}");
                        }
                        else
                        {
                            Console.WriteLine($"[Kết quả] Người chiến thắng: {gameEnd.WinnerName}");
                            Console.WriteLine($"[Kết quả] Lý do: {gameEnd.reason}");
                        }
                        Console.WriteLine("========================================\n");
                        Console.WriteLine("[Hệ thống] Trở lại sảnh chờ. Sẵn sàng nhận lời thách đấu.");
                        _currentRoomId = "";
                        _isMyTurn = false;
                    }
                    break;

                case PacketType.TimerExpired:
                    var timerExpired = JsonSerializer.Deserialize<TimerExpiredDTO>(packet.payload);
                    if (timerExpired != null)
                    {
                        Console.WriteLine("\n========================================");
                        Console.WriteLine($"[Kết quả] HẾT GIỜ!");
                        Console.WriteLine($"[Kết quả] {timerExpired.message}");
                        Console.WriteLine("========================================\n");
                        Console.WriteLine("[Hệ thống] Trở lại sảnh chờ. Sẵn sàng nhận lời thách đấu.");
                        _currentRoomId = "";
                        _isMyTurn = false;
                    }
                    break;

                case PacketType.OnlinePlayerList:
                    var onlinePlayers = JsonSerializer.Deserialize<OnlinePlayerListDTO>(packet.payload);
                    if (onlinePlayers != null)
                    {
                        Console.WriteLine($"[Lobby] Số người chơi trực tuyến: {onlinePlayers.players.Count}");
                    }
                    break;
            }
        }

        private static async Task MakeMoveAsync()
        {
            if (!_isMyTurn || string.IsNullOrEmpty(_currentRoomId)) return;

            Console.WriteLine("[AI] Đang tính toán nước đi tối ưu (Minimax Depth=4)...");
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var boardState = _aiEngine.GetBoardState();

            // Gọi thuật toán tìm kiếm Minimax với độ sâu 4
            var result = await Task.Run(() => AIServices.MiniMax(boardState, 4, int.MinValue, int.MaxValue, true, _ourPlayerId));

            watch.Stop();

            int finalRow = 7;
            int finalCol = 7;

            if (result.move.HasValue)
            {
                finalRow = result.move.Value.r;
                finalCol = result.move.Value.c;
            }
            else
            {
                // Fallback nếu không tính toán được nước đi
            }

            Console.WriteLine($"[AI] Quyết định đi ô [{finalRow}, {finalCol}] (Thời gian tính toán: {watch.ElapsedMilliseconds} ms, Điểm đánh giá: {result.score})");

            var moveReq = new MoveRequestDTO
            {
                row = finalRow,
                col = finalCol
            };

            await SendPacketAsync(PacketType.MoveRequest, moveReq);
            _isMyTurn = false;
        }
    }
}