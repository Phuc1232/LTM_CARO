using CaroGame.Client.Network;
using CaroGame.Client.Network;
using CaroGame.Client.UI_Components;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CaroGame.Client.Forms
{
    public class GameBoard : Form
    {
        private CaroBoard _board = new CaroBoard();

        private TcpClientManager _network = new TcpClientManager();

        private int _currentPlayer = 1; // 1 = X, 2 = O

        public GameBoard()
        {
            this.Text = "Caro Game - Lượt của X";

            this.Size = new Size(500, 550);

            // Setup board
            _board.Location = new Point(10, 10);

            _board.CellClicked += OnCellClicked;

            this.Controls.Add(_board);

            // Network events
            _network.OnConnected += () =>
            {
                MessageBox.Show("Connected to server");
            };

            _network.OnDisconnected += () =>
            {
                MessageBox.Show("Disconnected from server");
            };

            _network.OnMessageReceived += (msg) =>
            {
                MessageBox.Show($"Server: {msg}");
            };

            // Connect server
            _ = _network.Connect("127.0.0.1", 5000);
        }

        private async void OnCellClicked(object? sender, Point e)
        {
            // Update local board
            _board.UpdateCell(e.X, e.Y, _currentPlayer);

            // Send move to server
            await _network.Send($"{e.X},{e.Y}");

            // Change turn
            _currentPlayer = (_currentPlayer == 1) ? 2 : 1;

            this.Text = (_currentPlayer == 1)
                ? "Caro Game - Lượt của X"
                : "Caro Game - Lượt của O";
        }
    }
}