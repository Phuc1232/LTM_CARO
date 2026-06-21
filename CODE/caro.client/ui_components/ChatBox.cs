using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace caro.client.ui_components
{
    public partial class ChatBox : UserControl
    {
        public event Action<string>? OnSendMessage;

        public ChatBox()
        {
            InitializeComponent();

            btnSend.Click += BtnSend_Click;
            txtMessage.KeyDown += TxtMessage_KeyDown;
            txtMessage.Enter += TxtMessage_Enter;
            txtMessage.Leave += TxtMessage_Leave;
        }

        private void BtnSend_Click(object? sender, EventArgs e)
        {
            SendMessage();
        }

        private void TxtMessage_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Chặn tiếng bíp mặc định
                SendMessage();
            }
        }

        private void TxtMessage_Enter(object? sender, EventArgs e)
        {
            if (txtMessage.Text == "Nhập Chat Ở Đây Lèeee")
            {
                txtMessage.Text = "";
                txtMessage.ForeColor = Color.White;
            }
        }

        private void TxtMessage_Leave(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                txtMessage.Text = "Nhập Chat Ở Đây Lèeee";
                txtMessage.ForeColor = Color.Gray;
            }
        }

        private void SendMessage()
        {
            string message = txtMessage.Text.Trim();
            if (!string.IsNullOrEmpty(message) && message != "Nhập Chat Ở Đây Lèeee")
            {
                OnSendMessage?.Invoke(message);
                txtMessage.Clear();
            }
        }

        public void AddMessage(string sender, string message)
        {
            if (rtbMessages.InvokeRequired)
            {
                rtbMessages.Invoke(() => AddMessage(sender, message));
                return;
            }
            rtbMessages.AppendText($"[{sender}]: {message}{Environment.NewLine}");
            rtbMessages.ScrollToCaret();
        }

        private void ChatBox_Load(object sender, EventArgs e)
        {

        }
    }
}
