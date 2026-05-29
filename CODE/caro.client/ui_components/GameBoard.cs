using CaroGame.Client.Network;
using CaroGame.Client.UI_Components;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CaroGame.Client.Forms
{
    public class GameBoard : Form
    {
        private CaroBoard _board =
            new CaroBoard();

        private TcpClientManager _network =
            new TcpClientManager();

        public GameBoard()
        {
            this.Text = "Caro Network Test";

            this.Size = new Size(500, 550);

            // Setup board
            _board.Location = new Point(10, 10);

            // Event click
            _board.CellClicked += OnCellClicked;

            this.Controls.Add(_board);

            // Connected
            _network.OnConnected += () =>
            {
                MessageBox.Show("Connected to server");
            };

            // Disconnected
            _network.OnDisconnected += () =>
            {
                MessageBox.Show("Disconnected from server");
            };

            // Receive move
            _network.OnMoveReceived += (move) =>
            {
                MessageBox.Show(
                    $"Move received:\nX = {move.X}\nY = {move.Y}");
            };

            // Receive raw message
            _network.OnMessageReceived += (sender, e) =>
            {
                Console.WriteLine("RAW: " + e.Message);
            };

            // Connect server
            _ = _network.Connect("127.0.0.1", 5000);
        }

        private async void OnCellClicked(object? sender, Point e)
        {
            // CHỈ TEST NETWORK
            // KHÔNG update UI nữa

            MoveMessage move = new MoveMessage
            {
                X = e.X,
                Y = e.Y
            };

            MessageBox.Show(
                $"Sending move:\nX = {move.X}\nY = {move.Y}");

            await _network.SendMove(move);
        }
    }
}