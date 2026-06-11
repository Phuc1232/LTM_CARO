using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using caro.client.network;
using caro.share.DTOs;
using caro.share.DTOs.Constants;

namespace caro.client.form
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            TCPClientManager.Instance.OnLoginResponse += HandleLoginResponse;
        }
        private void HandleLoginResponse(LoginResponseDTO response)
        {
            if (InvokeRequired)
            {
                Invoke(() => HandleLoginResponse(response));
                return;
            }

            if (response.isSuccess)
            {
                MessageBox.Show("Đăng nhập thành công!");

                Home home = new Home();
                home.Show();

                this.Hide();
            }
            else
            {
                MessageBox.Show(response.message);
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string ip = txtIpAddress.Text.Trim();

            if (username == "" || ip == "")
            {
                MessageBox.Show("Vui lòng nhập Username và IP Address!");
                return;
            }
            bool connected = await TCPClientManager.Instance.ConnectAsync(ip, 8888);

            if (connected)
            {
                var loginRequest = new LoginRequestDTO
                {
                    username = username
                };

                await TCPClientManager.Instance.SendPacketAsync(
                    PacketType.LoginRequest,
                    loginRequest
                );

                MessageBox.Show("Đã gửi yêu cầu đăng nhập lên server!");

       
            }
            else
            {
                MessageBox.Show(
           "Không thể kết nối server. Hãy kiểm tra IP hoặc mở server trước!",
           "Connection Error",
           MessageBoxButtons.OK,
           MessageBoxIcon.Error);
            }
        }
    }
}
