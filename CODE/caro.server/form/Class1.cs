using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public class SimpleServer
{
    private TcpListener _listener;
    private bool _isRunning;
    private CancellationTokenSource _cts;

    // Các Event để gửi dữ liệu về UI
    public event Action<string> OnLogReceived;
    public event Action<string, bool> OnPlayerStatusChanged; // string: PlayerIP/Name, bool: isConnect

    public void Start(int port)
    {
        if (_isRunning) return;

        _isRunning = true;
        _cts = new CancellationTokenSource();
        _listener = new TcpListener(IPAddress.Any, port);
        _listener.Start();

        Log("Server đã khởi động trên port " + port);

        // Chạy vòng lặp chấp nhận client trên một Thread/Task khác để không làm đơ UI
        Task.Run(() => ListenForClients(_cts.Token));
    }

    public void Stop()
    {
        if (!_isRunning) return;

        _isRunning = false;
        _cts?.Cancel();
        _listener?.Stop();
        Log("Server đã dừng.");
    }

    private async Task ListenForClients(CancellationToken token)
    {
        while (_isRunning && !token.IsCancellationRequested)
        {
            try
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();
                string clientEndPoint = client.Client.RemoteEndPoint.ToString();

                Log($"Client kết nối từ: {clientEndPoint}");
                OnPlayerStatusChanged?.Invoke(clientEndPoint, true);

                // Tạm thời chưa xử lý DTOs (để dành cho tuần 3), chỉ giữ kết nối hoặc xử lý cơ bản ở đây
            }
            catch (Exception)
            {
                // Khi Stop Server, AcceptTcpClientAsync sẽ ném ngoại lệ, chúng ta catch để tránh crash
                break;
            }
        }
    }

    private void Log(string message)
    {
        OnLogReceived?.Invoke($"[{DateTime.Now:HH:mm:ss}] {message}");
    }
   
}