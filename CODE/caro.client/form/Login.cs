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
            TCPClientManager.Instance.OnDisconnected += HandleDisconnected;
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
                

                Home home = new Home();
                home.Show();

                this.Hide();
            }
            else
            {
                MessageBox.Show(response.message);
            }
        }
        private void HandleDisconnected()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(HandleDisconnected));
                return;
            }

            
            if (!this.Visible)
            {
                MessageBox.Show("Mất kết nối tới máy chủ! Hệ thống sẽ quay lại trang đăng nhập.", "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);

                
                this.Show();

               
                List<Form> formsToClose = new List<Form>();
                foreach (Form form in Application.OpenForms)
                {
                    if (form != this)
                    {
                        formsToClose.Add(form);
                    }
                }

                foreach (Form form in formsToClose)
                {
                    form.Close();
                }
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string ip = txtIpAddress.Text.Trim();
            TCPClientManager.Instance.CurrentUsername = username;

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
