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
    public partial class Form1 : Form
    {
        private Form2 _form2;
        private ServerManager _serverManager;
        public Form1()
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
            // 1. Khởi tạo một đối tượng của Form2
            Form2 f2 = new Form2();

            // 2. Hiển thị Form2 lên màn hình
            f2.Show();

            // 3. Ẩn Form1 hiện tại đi
            this.Hide();

            // LƯU Ý QUAN TRỌNG: Để tránh ứng dụng bị chạy ngầm khi người dùng tắt Form2 bằng dấu X
            f2.FormClosed += (s, args) => Application.Exit();
            if (!int.TryParse(txtPort1.Text, out int port)) 
            {
                MessageBox.Show("Port không hợp lệ!");
                return;
            }

            if (_form2 == null || _form2.IsDisposed)
            {
                _form2 = new Form2();
                _form2.Show();
            }
            _serverManager.StartServer(port);


            btnStart.Enabled = false;
            btnStop.Enabled = true;

            _form2.UpdateStatus($"Đang mở tại Port: {port}");

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
