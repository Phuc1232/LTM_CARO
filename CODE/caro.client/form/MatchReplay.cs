using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using caro.share.DTOs;

namespace caro.client.form
{
    public partial class MatchReplay : Form
    {
        private readonly MatchHistoryItemDTO _match;
        private readonly List<ReplayMove> _moves = new();
        private readonly Button[,] _cells = new Button[15, 15];
        private int _currentStep = 0;

        public MatchReplay(MatchHistoryItemDTO match)
        {
            InitializeComponent();

            _match = match;

            Text = "Xem lại trận đấu";
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(30, 30, 46);
            ClientSize = new Size(760, 760);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            ParseMoves();
            BuildUI();
            RenderBoard();
        }

        private void ParseMoves()
        {
            _moves.Clear();

            if (string.IsNullOrWhiteSpace(_match.MovesData))
                return;

            string[] moveParts = _match.MovesData.Split(';', StringSplitOptions.RemoveEmptyEntries);

            foreach (string moveText in moveParts)
            {
                string[] parts = moveText.Split(':');
                if (parts.Length != 2) continue;

                string player = parts[0];

                string[] position = parts[1].Split(',');
                if (position.Length != 2) continue;

                if (!int.TryParse(position[0], out int row)) continue;
                if (!int.TryParse(position[1], out int col)) continue;

                _moves.Add(new ReplayMove
                {
                    Player = player,
                    Row = row,
                    Col = col
                });
            }
        }

        private void BuildUI()
        {
            Label lblTitle = new Label
            {
                Text = $"{_match.Player1} vs {_match.Player2}",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 20)
            };
            Controls.Add(lblTitle);

            Label lblInfo = new Label
            {
                Name = "lblInfo",
                Text = "",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(30, 60)
            };
            Controls.Add(lblInfo);

            Panel boardPanel = new Panel
            {
                Location = new Point(30, 100),
                Size = new Size(600, 600)
            };
            Controls.Add(boardPanel);

            int cellSize = 40;

            for (int row = 0; row < 15; row++)
            {
                for (int col = 0; col < 15; col++)
                {
                    Button btn = new Button
                    {
                        Width = cellSize,
                        Height = cellSize,
                        Left = col * cellSize,
                        Top = row * cellSize,
                        Font = new Font("Segoe UI", 12, FontStyle.Bold),
                        BackColor = Color.FromArgb(42, 42, 60),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Enabled = true
                    };

                    btn.FlatAppearance.BorderSize = 1;
                    btn.FlatAppearance.BorderColor = Color.FromArgb(90, 90, 120);

                    _cells[row, col] = btn;
                    boardPanel.Controls.Add(btn);
                }
            }

            Button btnPrev = new Button
            {
                Text = "Lùi",
                Size = new Size(90, 45),
                Location = new Point(650, 180)
            };
            btnPrev.Click += (s, e) =>
            {
                if (_currentStep > 0)
                {
                    _currentStep--;
                    RenderBoard();
                }
            };
            Controls.Add(btnPrev);

            Button btnNext = new Button
            {
                Text = "Tiếp",
                Size = new Size(90, 45),
                Location = new Point(650, 240)
            };
            btnNext.Click += (s, e) =>
            {
                if (_currentStep < _moves.Count)
                {
                    _currentStep++;
                    RenderBoard();
                }
            };
            Controls.Add(btnNext);

            Button btnRestart = new Button
            {
                Text = "Chơi lại",
                Size = new Size(90, 45),
                Location = new Point(650, 300)
            };
            btnRestart.Click += (s, e) =>
            {
                _currentStep = 0;
                RenderBoard();
            };
            Controls.Add(btnRestart);

            Button btnClose = new Button
            {
                Text = "Đóng",
                Size = new Size(90, 45),
                Location = new Point(650, 360)
            };
            btnClose.Click += (s, e) => Close();
            Controls.Add(btnClose);
        }

        private void RenderBoard()
        {
            for (int row = 0; row < 15; row++)
            {
                for (int col = 0; col < 15; col++)
                {
                    _cells[row, col].Text = "";
                }
            }

            for (int i = 0; i < _currentStep; i++)
            {
                ReplayMove move = _moves[i];

                if (move.Row < 0 || move.Row >= 15 || move.Col < 0 || move.Col >= 15)
                    continue;

                _cells[move.Row, move.Col].Text = i % 2 == 0 ? "X" : "O";
                _cells[move.Row, move.Col].ForeColor = Color.White;
                _cells[move.Row, move.Col].Font = new Font("Segoe UI", 18, FontStyle.Bold);
                _cells[move.Row, move.Col].BackColor = Color.FromArgb(42, 42, 60);
                _cells[move.Row, move.Col].UseVisualStyleBackColor = false;
                _cells[move.Row, move.Col].Enabled = true;
            }
            if (_currentStep == _moves.Count)
            {
                List<Point> winningCells = FindWinningCells();

                foreach (Point p in winningCells)
                {
                    _cells[p.X, p.Y].BackColor = Color.Gold;
                    _cells[p.X, p.Y].ForeColor = Color.Black;
                    _cells[p.X, p.Y].UseVisualStyleBackColor = false;
                }
            }

            Label? lblInfo = Controls.Find("lblInfo", true).FirstOrDefault() as Label;

            if (lblInfo != null)
            {
                string winner = string.IsNullOrEmpty(_match.Winner) ? "Hòa" : _match.Winner;

                lblInfo.Text =
                    $"Kết quả: {winner} | " +
                    $"Bước: {_currentStep}/{_moves.Count} | " +
                    $"Ngày: {_match.PlayedAt.ToLocalTime():dd/MM/yyyy HH:mm}";
            }
        }

        private class ReplayMove
        {
            public string Player { get; set; } = "";
            public int Row { get; set; }
            public int Col { get; set; }
        }
    }
}