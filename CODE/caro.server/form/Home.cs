using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace caro.server.form
{
    public partial class Home : Form
    {
        private Log _form2;
        private ServerManager _serverManager;
        public Home()
        {
            InitializeComponent();
            _serverManager = new ServerManager();
            _serverManager.OnLogMessage += HandleServerLog;
            _serverManager.OnPlayerConnectionChanged += HandlePlayerConnection;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _serverManager.StopServer();

            btnStart.Enabled = true;
            btnStop.Enabled = false;

            if (_form2 != null && !_form2.IsDisposed)
            {
                _form2.UpdateStatus("Server đang đóng.");
            }
        }
        private void HandleServerLog(string message)
        {
            if (_form2 != null && !_form2.IsDisposed)
            {
                _form2.AppendLog(message);
            }
        }
        private void HandlePlayerConnection(string playerInfo, bool isConnecting)
        {
            if (_form2 != null && !_form2.IsDisposed)
            {
                _form2.UpdatePlayerList(playerInfo, isConnecting);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra Port hợp lệ trước khi chuyển Form
            if (!int.TryParse(txtPort1.Text, out int port)) 
            {
                MessageBox.Show("Port không hợp lệ!");
                return;
            }

            // 2. Khởi tạo duy nhất một đối tượng của Form2 lưu vào biến toàn cục _form2
            if (_form2 == null || _form2.IsDisposed)
            {
                _form2 = new Log();
                // LƯU Ý QUAN TRỌNG: Để tránh ứng dụng bị chạy ngầm khi người dùng tắt Form2 bằng dấu X
                _form2.ParentForm1 = this;
                _form2.FormClosed += (s, args) => Application.Exit();
            }

            // 3. Hiển thị Form2 lên màn hình
            _form2.Show();

            // 4. Ẩn Form1 hiện tại đi
            this.Hide();

            // 5. Khởi động server và cập nhật trạng thái
            _serverManager.StartServer(port);

            btnStart.Enabled = false;
            btnStop.Enabled = true;

            _form2.UpdateStatus($"Đang mở tại Port: {port}");
        }
        // Hàm này giúp Form2 có thể gọi ngược lại Form1 để dừng Server và hiển thị Form1 lên
        public void StopServerAndShow()
        {
            // 1. Dừng server
            _serverManager.StopServer();

            // 2. Reset trạng thái các nút bấm trên Form1 về trạng thái ban đầu
            btnStart.Enabled = true;
            btnStop.Enabled = false;

            // 3. Hiển thị lại Form1
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
//Test-NetConnection -ComputerName localhost -Port 8888
