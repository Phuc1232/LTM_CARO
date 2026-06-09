using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using caro.server.network; // Đảm bảo import đúng namespace chứa TCPServerManager

namespace caro.server.form
{
    public partial class Home : Form
    {
        private Log _Log;
        private TCPServerManager _serverManager; // Sử dụng TCPServerManager thực tế

        public Home()
        {
            InitializeComponent();
            _serverManager = new TCPServerManager(); // Khởi tạo đối tượng TCPServerManager

            // Đăng ký nhận sự kiện tĩnh từ TCPServerManager để đẩy dữ liệu log/status về UI
            TCPServerManager.OnLogMessage += HandleServerLog;
            TCPServerManager.OnPlayerConnectionChanged += HandlePlayerConnection;
        }

        private void BTN_StopServer(object sender, EventArgs e)
        {
            _serverManager.StopServer();
            Application.Exit(); // Thoát hoàn toàn ứng dụng
        }

        private void HandleServerLog(string message)
        {
            if (_Log != null && !_Log.IsDisposed)
            {
                _Log.AppendLog(message);
            }
        }

        private void HandlePlayerConnection(string playerInfo, bool isConnecting)
        {
            if (_Log != null && !_Log.IsDisposed)
            {
                _Log.UpdatePlayerList(playerInfo, isConnecting);
            }
        }

        private void BTN_StartServer(object sender, EventArgs e)
        {
            // 1. Khởi tạo duy nhất một đối tượng của Form2 (Log) lưu vào biến toàn cục _Log
            if (_Log == null || _Log.IsDisposed)
            {
                _Log = new Log();
                _Log.ParentForm1 = this;
                _Log.FormClosed += (s, args) => Application.Exit();
            }

            // 2. Hiển thị Form2 lên màn hình
            _Log.Show();

            // 3. Ẩn Form Home hiện tại đi
            this.Hide();

            // 4. Khởi động server với port cố định là 8888 theo yêu cầu
            int port = 8888;
            _serverManager.StartServer(port);

            btnStart.Enabled = false;
            btnStop.Enabled = true;

            //_Log.UpdateStatus($"Đang mở tại Port: {port}");
        }

        // Hàm giúp Form2 gọi ngược lại Form Home khi người dùng click tắt/dừng server
        public void StopServerAndShow()
        {
            // 1. Dừng server
            _serverManager.StopServer();

            // 2. Reset trạng thái các nút bấm trên Form Home
            btnStart.Enabled = true;
            btnStop.Enabled = true;

            // 3. Hiển thị lại Form Home
            this.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void txtPort1_TextChanged(object sender, EventArgs e)
        {
        }
    }
}
