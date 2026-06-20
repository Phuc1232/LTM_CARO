using System;
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
        }

        private void BtnSend_Click(object? sender, EventArgs e)
        {
            SendCurrentMessage();
        }

        private void TxtMessage_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SendCurrentMessage();
            }
        }

        private void SendCurrentMessage()
        {
            string message = txtMessage.Text.Trim();

            if (string.IsNullOrEmpty(message)) return;
            if (message == "Nhập Chat Ở Đây Lèeee") return;

            OnSendMessage?.Invoke(message);
            txtMessage.Clear();
        }

        public void AddMessage(string username, string message)
        {
            rtbMessages.AppendText($"{username}: {message}\n");
            rtbMessages.ScrollToCaret();
        }

        private void ChatBox_Load(object sender, EventArgs e)
        {
        }
    }
}