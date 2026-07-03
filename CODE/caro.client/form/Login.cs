using caro.client.network;
using caro.share.DTOs;
using caro.share.DTOs.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace caro.client.form
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            TCPClientManager.Instance.OnLoginResponse += HandleLoginResponse;
            TCPClientManager.Instance.OnDisconnected += HandleDisconnected;
            ApplyThemeColors();
        }

        private void ApplyThemeColors()
        {
            this.BackColor = UITheme.FormBackColor;
            this.ForeColor = UITheme.TextForeColor;

            label1.BackColor = Color.Transparent;
            label1.ForeColor = UITheme.TitleColor;
            label1.Font = new Font("Segoe UI Semibold", 24F, FontStyle.Bold);

            Label2.BackColor = Color.Transparent;
            Label2.ForeColor = UITheme.TextForeColor;
            Label2.Font = new Font("Segoe UI", 12F);

            Label3.BackColor = Color.Transparent;
            Label3.ForeColor = UITheme.TextForeColor;
            Label3.Font = new Font("Segoe UI", 12F);

            txtUsername.BackColor = UITheme.InputBackColor;
            txtUsername.ForeColor = UITheme.InputForeColor;
            txtUsername.BorderStyle = BorderStyle.FixedSingle;
            txtUsername.Font = new Font("Segoe UI", 12F);

            txtIpAddress.BackColor = UITheme.InputBackColor;
            txtIpAddress.ForeColor = UITheme.InputForeColor;
            txtIpAddress.BorderStyle = BorderStyle.FixedSingle;
            txtIpAddress.Font = new Font("Segoe UI", 12F);

            if (btnLogin != null)
            {
                btnLogin.ApplyThemeColors();
            }
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
                TCPClientManager.Instance.CurrentUsername = txtUsername.Text.Trim();
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
