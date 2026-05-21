using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using caro.server.Services;

public class ServerManager
{
    private TcpListener _listener;
    private bool _isRunning;
    private ClientManager _clientManager = new ClientManager();
    // Các delegate để truyền dữ liệu về Form
    public Action<string> OnLogMessage;
    public Action<string, bool> OnPlayerConnectionChanged; // string: ID/IP người chơi, bool: true (connect), false (disconnect)
    private readonly object _form2;

    public void StartServer(int port)
    {
        try
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            _isRunning = true;

            OnLogMessage?.Invoke($"[Network] Server bắt đầu lắng nghe tại Port {port}");

            // Bắt đầu vòng lặp lắng nghe client trên một luồng (thread/task) khác
            Task.Run(() => AcceptClientsAsync());
        }
        catch (Exception ex)
        {
            OnLogMessage?.Invoke($"[Error] Không thể khởi động server: {ex.Message}");
        }
    }

    public void StopServer()
    {
        _isRunning = false;
        _listener?.Stop();
        _clientManager.DisconnectAll();
        OnLogMessage?.Invoke("[Network] Đã dừng server.");
    }

    private async Task AcceptClientsAsync()
    {
        while (_isRunning)
        {
            try
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();
                string clientInfo = client.Client.RemoteEndPoint.ToString();
                _clientManager.AddClient(clientInfo, client);
                // Báo cáo có client kết nối (Hiển thị connect)
                OnLogMessage?.Invoke($"[Client] {clientInfo} đã kết nối. Tổng online: {_clientManager.GetOnlineCount()}");
                OnPlayerConnectionChanged?.Invoke(clientInfo, true);

                // TODO ở Tuần 3-4: Quản lý client này vào Dictionary và tạo luồng nhận dữ liệu
            }
            catch (ObjectDisposedException)
            {
                // Bỏ qua lỗi này khi StopServer được gọi làm ngắt listener
                break;
            }
            catch (Exception ex)
            {
                if (_isRunning) OnLogMessage?.Invoke($"[Error] Lỗi khi nhận client: {ex.Message}");
            }
        }
    }
    
}