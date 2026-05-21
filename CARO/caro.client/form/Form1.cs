using System;
using System.Windows.Forms;
using CaroGame.Client.Network;

namespace caro.client
{
    public class Form1 : Form
    {
        private TcpClientManager _network = new TcpClientManager();

        public Form1()
        {
            Text = "TCP Client Test";

            Width = 400;

            Height = 300;

            Load += Form1_Load;
        }

        private async void Form1_Load(object? sender, EventArgs e)
        {
            _network.OnConnected += () =>
            {
                MessageBox.Show("Connected");
            };

            _network.OnMessageReceived += (msg) =>
            {
                MessageBox.Show($"Server: {msg}");
            };

            await _network.Connect("127.0.0.1", 5000);

            await _network.Send("HELLO SERVER");
        }
    }
}