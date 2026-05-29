using caro.server.services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace caro.server.network
{
    public class TCPServerManager
    {
        // Danh sách lưu trữ người chơi online dạng thread-safe
        public static readonly ConcurrentDictionary<string, ClientHandle> onlineplayer = new();

        private static TcpListener _listener;
        private static bool _isRunning;
        private static CancellationTokenSource _cts;

        // Các sự kiện tĩnh đẩy dữ liệu về giao diện (UI) để cập nhật logs và danh sách người chơi
        public static event Action<string> OnLogMessage;
        public static event Action<string, bool> OnPlayerConnectionChanged; // string: tên người chơi, bool: true = online, false = offline

        /// <summary>
        /// Khởi chạy TCP Server trên port cố định (mặc định là 8888).
        /// </summary>
        public void StartServer(int port = 8888)
        {
            if (_isRunning) return;

            _isRunning = true;
            _cts = new CancellationTokenSource();
            _listener = new TcpListener(IPAddress.Any, port);
            
            try
            {
                _listener.Start();
                Log($"Server đã khởi động thành công trên cổng: {port}");
                _ = Task.Run(async () =>
                {
                    Log("[Database] Đang khởi tạo kết nối PostgreSQL (EF Core Code-First)...");
                    bool isSuccess = await DatabaseServices.Instance.InitializeDatabaseAsync();
                    if (isSuccess)
                    {
                        Log("[Database] Kết nối PostgreSQL thành công. Cơ cấu bảng lịch sử đấu sẵn sàng.");
                    }
                    else
                    {
                        Log("[Database] [Cảnh báo] Lỗi kết nối PostgreSQL. Hãy đảm bảo Docker Container đã chạy.");
                    }
                    // Chạy tác vụ lắng nghe client trên luồng phụ để tránh gây đơ/treo giao diện WinForms
                    _ = Task.Run(() => ListenForClientsAsync(_cts.Token));
                });
            }
            catch (Exception ex)
            {
                _isRunning = false;
                Log($"Lỗi khi khởi động Server: {ex.Message}");
            }
        }

        /// <summary>
        /// Dừng Server an toàn, ngắt kết nối toàn bộ client đang kết nối và giải phóng cổng mạng.
        /// </summary>
        public void StopServer()
        {
            if (!_isRunning) return;

            _isRunning = false;
            _cts?.Cancel();

            try
            {
                _listener?.Stop();
            }
            catch (Exception ex)
            {
                Log($"Lỗi khi dừng listener: {ex.Message}");
            }

            // Đóng kết nối của toàn bộ người chơi đang online một cách an toàn
            foreach (var handler in onlineplayer.Values)
            {
                try
                {
                    handler.CloseConnection(); // Hàm đóng kết nối
                }
                catch (Exception ex)
                {
                    Log($"Lỗi khi ngắt kết nối client: {ex.Message}");
                }
            }

            onlineplayer.Clear();
            Log("Server đã dừng hoạt động.");
        }

        /// <summary>
        /// Vòng lặp bất đồng bộ lắng nghe và chấp nhận các kết nối TCP client mới.
        /// </summary>
        private async Task ListenForClientsAsync(CancellationToken token)
        {
            while (_isRunning && !token.IsCancellationRequested)
            {
                try
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    
                    // Tạo một đối tượng xử lý kết nối cho client mới
                    ClientHandle handler = new ClientHandle(client);
                    _ = Task.Run(() => handler.HandleClientAsync(), token);
                }
                catch (ObjectDisposedException)
                {
                    // Lỗi xảy ra khi Stop listener, chúng ta chủ động bắt lỗi để thoát vòng lặp
                    break;
                }
                catch (Exception ex)
                {
                    if (_isRunning)
                    {
                        Log($"Lỗi khi chấp nhận kết nối: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Hàm ghi log tập trung đẩy về giao diện UI
        /// </summary>
        public static void Log(string message)
        {
            OnLogMessage?.Invoke(message);
        }

        /// <summary>
        /// Hàm cập nhật trạng thái online/offline người chơi đẩy về giao diện UI
        /// </summary>
        public static void ChangePlayerStatus(string username, bool isConnecting)
        {
            OnPlayerConnectionChanged?.Invoke(username, isConnecting);
        }
    }
}