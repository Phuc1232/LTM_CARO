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

        // Player hiện tại
        private int _currentPlayer = 1;

        public GameBoard()
        {
            this.Text =
                "Caro Game - Lượt của X";

            this.Size =
                new Size(500, 550);

            // Setup board
            _board.Location =
                new Point(10, 10);

            _board.CellClicked +=
                OnCellClicked;

            this.Controls.Add(_board);

            // Connected
            _network.OnConnected += () =>
            {
                MessageBox.Show(
                    "Connected to server");
            };

            // Disconnected
            _network.OnDisconnected += () =>
            {
                MessageBox.Show(
                    "Disconnected from server");
            };

            // Receive move từ server
            _network.OnMoveReceived +=
                (move) =>
                {
                    this.Invoke(() =>
                    {
                        // Draw quân đối thủ
                        _board.UpdateCell(
                            move.X,
                            move.Y,
                            2);

                        MessageBox.Show(
                            $"Move received: X={move.X}, Y={move.Y}");
                    });
                };

            // Receive chat
            _network.OnChatReceived +=
                (chat) =>
                {
                    MessageBox.Show(
                        $"Chat: {chat.Message}");
                };

            // Receive status
            _network.OnStatusReceived +=
                (status) =>
                {
                    MessageBox.Show(
                        $"Status: {status.Status}");
                };

            // Raw message
            _network.OnMessageReceived +=
                (msg) =>
                {
                    Console.WriteLine(
                        $"RAW: {msg}");
                };

            // Connect server
            _ = _network.Connect(
                "127.0.0.1",
                5000);
        }

        private async void OnCellClicked(
            object? sender,
            Point e)
        {
            // Draw local move
            _board.UpdateCell(
                e.X,
                e.Y,
                1);

            // Create move message
            MoveMessage move =
                new MoveMessage
                {
                    X = e.X,
                    Y = e.Y
                };

            // Send move
            await _network.SendMove(move);

            Console.WriteLine(
                $"SEND: X={e.X}, Y={e.Y}");

            // Change title
            this.Text =
                "Caro Game - Đã gửi nước đi";
        }
    }
}