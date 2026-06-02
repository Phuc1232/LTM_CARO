using System;
using System.Drawing;
using System.Windows.Forms;
using caro.client.network;
using caro.share.DTOs;
using caro.share.DTOs.Constants;
using CaroGame.Client.Forms;

namespace caro.client
{
    public partial class Form1 : Form
    {
        // Điều phối giao diện lập trình (Programmatic UI)
        private Panel pnlLogin = null!;
        private TextBox txtUsername = null!;
        private TextBox txtIP = null!;
        private TextBox txtPort = null!;
        private Button btnLogin = null!;

        private Panel pnlLobby = null!;
        private Label lblWelcome = null!;
        private ListBox lstOnlinePlayers = null!;
        private Button btnChallenge = null!;

        public Form1()
        {
            InitializeComponent();
            SetupCustomUI();

            // Đăng ký nhận sự kiện từ Network Layer của Client
            TCPClientManager.Instance.OnLoginResponse += HandleLoginResponse;
            TCPClientManager.Instance.OnOnlinePlayerListUpdated += HandleOnlinePlayerListUpdated;
            TCPClientManager.Instance.OnChallengeReceived += HandleChallengeReceived;
            TCPClientManager.Instance.OnChallengeResult += HandleChallengeResult;
            TCPClientManager.Instance.OnGameStarted += HandleGameStarted;
            TCPClientManager.Instance.OnDisconnected += HandleDisconnected;
        }

        private void SetupCustomUI()
        {
            // Thiết lập Form chính
            this.Text = "Caro Online - Lập trình mạng";
            this.Size = new Size(800, 500);
            this.BackColor = Color.FromArgb(30, 30, 47); // Dark Theme hiện đại
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // 1. THIẾT LẬP PANEL ĐĂNG NHẬP (pnlLogin)
            pnlLogin = new Panel
            {
                Size = new Size(400, 350),
                Location = new Point((this.ClientSize.Width - 400) / 2, (this.ClientSize.Height - 350) / 2),
                BackColor = Color.FromArgb(43, 43, 64),
                BorderStyle = BorderStyle.None
            };

            // Tiêu đề Login
            Label lblLoginTitle = new Label
            {
                Text = "CARO ONLINE",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 191, 255), // Deep sky blue
                Size = new Size(360, 45),
                Location = new Point(20, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlLogin.Controls.Add(lblLoginTitle);

            // Nhập Username
            Label lblUser = new Label { Text = "Tên người chơi:", ForeColor = Color.White, Location = new Point(40, 90), Size = new Size(320, 20), Font = new Font("Segoe UI", 10, FontStyle.Regular) };
            txtUsername = new TextBox { Location = new Point(40, 115), Size = new Size(320, 30), Font = new Font("Segoe UI", 12), BackColor = Color.FromArgb(30, 30, 47), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            txtUsername.Text = "Player_" + new Random().Next(100, 999);
            pnlLogin.Controls.Add(lblUser);
            pnlLogin.Controls.Add(txtUsername);

            // Nhập IP
            Label lblIP = new Label { Text = "Địa chỉ Server IP:", ForeColor = Color.White, Location = new Point(40, 160), Size = new Size(150, 20), Font = new Font("Segoe UI", 10, FontStyle.Regular) };
            txtIP = new TextBox { Location = new Point(40, 185), Size = new Size(150, 30), Font = new Font("Segoe UI", 12), BackColor = Color.FromArgb(30, 30, 47), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            txtIP.Text = "127.0.0.1";
            pnlLogin.Controls.Add(lblIP);
            pnlLogin.Controls.Add(txtIP);

            // Nhập Port
            Label lblPort = new Label { Text = "Cổng Port:", ForeColor = Color.White, Location = new Point(210, 160), Size = new Size(150, 20), Font = new Font("Segoe UI", 10, FontStyle.Regular) };
            txtPort = new TextBox { Location = new Point(210, 185), Size = new Size(150, 30), Font = new Font("Segoe UI", 12), BackColor = Color.FromArgb(30, 30, 47), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            txtPort.Text = "8888";
            pnlLogin.Controls.Add(lblPort);
            pnlLogin.Controls.Add(txtPort);

            // Nút đăng nhập
            btnLogin = new Button
            {
                Text = "KẾT NỐI & ĐĂNG NHẬP",
                Location = new Point(40, 250),
                Size = new Size(320, 45),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(39, 174, 96), // Emerald Green
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.MouseEnter += (s, e) => btnLogin.BackColor = Color.FromArgb(46, 204, 113);
            btnLogin.MouseLeave += (s, e) => btnLogin.BackColor = Color.FromArgb(39, 174, 96);
            btnLogin.Click += BtnLogin_Click;
            pnlLogin.Controls.Add(btnLogin);

            this.Controls.Add(pnlLogin);


            // 2. THIẾT LẬP PANEL SẢNH CHỜ (pnlLobby)
            pnlLobby = new Panel
            {
                Size = new Size(760, 420),
                Location = new Point(12, 12),
                BackColor = Color.FromArgb(43, 43, 64),
                Visible = false // Mặc định ẩn
            };

            lblWelcome = new Label
            {
                Text = "Xin chào, người chơi!",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                Size = new Size(400, 30)
            };
            pnlLobby.Controls.Add(lblWelcome);

            Label lblOnline = new Label
            {
                Text = "Danh sách người chơi đang trực tuyến:",
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.Silver,
                Location = new Point(20, 60),
                Size = new Size(400, 20)
            };
            pnlLobby.Controls.Add(lblOnline);

            // ListBox người chơi online
            lstOnlinePlayers = new ListBox
            {
                Location = new Point(20, 90),
                Size = new Size(450, 300),
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(30, 30, 47),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            pnlLobby.Controls.Add(lstOnlinePlayers);

            // Nút Thách Đấu
            btnChallenge = new Button
            {
                Text = "GỬI LỜI THÁCH ĐẤU",
                Location = new Point(490, 90),
                Size = new Size(240, 50),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(230, 126, 34), // Orange
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnChallenge.FlatAppearance.BorderSize = 0;
            btnChallenge.MouseEnter += (s, e) => btnChallenge.BackColor = Color.FromArgb(243, 156, 18);
            btnChallenge.MouseLeave += (s, e) => btnChallenge.BackColor = Color.FromArgb(230, 126, 34);
            btnChallenge.Click += BtnChallenge_Click;
            pnlLobby.Controls.Add(btnChallenge);

            this.Controls.Add(pnlLobby);
        }

        private async void BtnLogin_Click(object? sender, EventArgs e)
        {
            string usernameInput = txtUsername.Text.Trim();
            string ipInput = txtIP.Text.Trim();
            string portStr = txtPort.Text.Trim();

            if (string.IsNullOrEmpty(usernameInput))
            {
                MessageBox.Show("Vui lòng điền tên đăng nhập!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(portStr, out int portVal))
            {
                MessageBox.Show("Cổng Port phải là số nguyên hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnLogin.Enabled = false;
            btnLogin.Text = "Đang kết nối...";

            bool isConnected = await TCPClientManager.Instance.ConnectAsync(ipInput, portVal);
            if (isConnected)
            {
                var req = new LoginRequestDTO { username = usernameInput };
                await TCPClientManager.Instance.SendPacketAsync(PacketType.LoginRequest, req);
            }
            else
            {
                MessageBox.Show("Không thể kết nối đến server. Vui lòng kiểm tra lại IP/Port hoặc đảm bảo Server đã chạy!", "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnLogin.Enabled = true;
                btnLogin.Text = "KẾT NỐI & ĐĂNG NHẬP";
            }
        }

        private async void BtnChallenge_Click(object? sender, EventArgs e)
        {
            if (lstOnlinePlayers.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một người chơi trong danh sách để thách đấu!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string target = lstOnlinePlayers.SelectedItem.ToString() ?? "";
            if (target == txtUsername.Text.Trim())
            {
                MessageBox.Show("Bạn không thể thách đấu chính mình!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnChallenge.Enabled = false;
            btnChallenge.Text = "Đang gửi yêu cầu...";

            var request = new ChallengeRequestDTO { targetUsername = target };
            await TCPClientManager.Instance.SendPacketAsync(PacketType.ChallengeRequest, request);
        }

        private void HandleLoginResponse(LoginResponseDTO response)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => HandleLoginResponse(response)));
                return;
            }

            btnLogin.Enabled = true;
            btnLogin.Text = "KẾT NỐI & ĐĂNG NHẬP";

            if (response.isSuccess)
            {
                lblWelcome.Text = $"Xin chào, {txtUsername.Text.Trim()}!";
                pnlLogin.Visible = false;
                pnlLobby.Visible = true;
            }
            else
            {
                MessageBox.Show($"Đăng nhập thất bại: {response.message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                TCPClientManager.Instance.Disconnect();
            }
        }

        private void HandleOnlinePlayerListUpdated(OnlinePlayerListDTO playerList)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => HandleOnlinePlayerListUpdated(playerList)));
                return;
            }

            lstOnlinePlayers.Items.Clear();
            string myUsername = txtUsername.Text.Trim();

            foreach (var player in playerList.players)
            {
                // Không hiển thị chính mình trong danh sách thách đấu
                if (player != myUsername)
                {
                    lstOnlinePlayers.Items.Add(player);
                }
            }
        }

        private void HandleChallengeReceived(ChallengeNotifyDTO challenge)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => HandleChallengeReceived(challenge)));
                return;
            }

            var confirm = MessageBox.Show(
                $"Người chơi '{challenge.fromUsername}' thách đấu bạn. Đồng ý tham gia?",
                "Lời mời thách đấu",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            var reply = new ChallengeResponseDTO
            {
                roomId = challenge.roomId,
                isAccepted = confirm == DialogResult.Yes
            };

            _ = TCPClientManager.Instance.SendPacketAsync(PacketType.ChallengeResponse, reply);
        }

        private void HandleChallengeResult(ChallengeResultDTO result)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => HandleChallengeResult(result)));
                return;
            }

            btnChallenge.Enabled = true;
            btnChallenge.Text = "GỬI LỜI THÁCH ĐẤU";

            if (!result.isAccepted)
            {
                MessageBox.Show(result.message, "Lời mời bị từ chối", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void HandleGameStarted(GameStartNotifyDTO gameStartInfo)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => HandleGameStarted(gameStartInfo)));
                return;
            }

            // Ẩn Form sảnh chờ
            this.Hide();

            // Mở GameBoard và truyền tham số tên người chơi
            GameBoard boardForm = new GameBoard(gameStartInfo.name_player1, gameStartInfo.name_player2, txtUsername.Text.Trim());
            boardForm.FormClosed += (s, args) =>
            {
                // Khi bàn cờ đóng, hiển thị lại sảnh chờ
                this.Show();
                btnChallenge.Enabled = true;
                btnChallenge.Text = "GỬI LỜI THÁCH ĐẤU";
            };
            boardForm.Show();
        }

        private void HandleDisconnected()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(HandleDisconnected));
                return;
            }

            MessageBox.Show("Đã ngắt kết nối khỏi máy chủ!", "Thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            
            pnlLobby.Visible = false;
            pnlLogin.Visible = true;
            btnLogin.Enabled = true;
            btnLogin.Text = "KẾT NỐI & ĐĂNG NHẬP";
        }
    }
}
