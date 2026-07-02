using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using caro.share.DTOs;

namespace caro.client.ui_components
{
    public partial class BoardControl : UserControl
    {
        private const int ROWS = 15;
        private const int COLS = 15;
        private const int CELL_SIZE = 40;

        public BoardControl()
        {
            InitializeComponent();
            CreateBoard();
            this.Width = COLS * CELL_SIZE;
            this.Height = ROWS * CELL_SIZE;
        }
        private void CreateBoard()
        {
            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLS; col++)
                {
                    Button cell = new Button();

                    cell.Width = CELL_SIZE;
                    cell.Height = CELL_SIZE;

                    cell.Left = col * CELL_SIZE;
                    cell.Top = row * CELL_SIZE;
                    cell.Text = "";
                    cell.FlatStyle = FlatStyle.Flat;
                    cell.FlatAppearance.BorderSize = 1;
                    cell.FlatAppearance.BorderColor = Color.FromArgb(90, 90, 120);

                    cell.Tag = $"{row},{col}";
                    cell.BackColor = Color.FromArgb(42, 42, 60);
                    cell.ForeColor = Color.White;
                    cell.UseVisualStyleBackColor = false;

                    cell.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                    cell.Margin = new Padding(0);
                    cell.Padding = new Padding(0);

                    cell.Click += Cell_Click;

                    Controls.Add(cell);
                }
            }
        }
        private bool _canPlay = true;
        private Button? _lastMoveButton = null;

        private void Cell_Click(object? sender, EventArgs e)
        {
            if (!_canPlay)
                return;

            Button cell = (Button)sender!;

            if (cell.Text != "")
                return;

            string[] parts = cell.Tag.ToString()!.Split(',');
            int row = int.Parse(parts[0]);
            int col = int.Parse(parts[1]);

            OnCellClicked?.Invoke(row, col);
        }
        public void SetCell(int row, int col, string text)
        {
            foreach (Control control in Controls)
            {
                if (control is Button btn && btn.Tag?.ToString() == $"{row},{col}")
                {
                    // X?a highlight c?a n??c ?i tr??c
                    if (_lastMoveButton != null)
                    {
                        _lastMoveButton.BackColor = Color.FromArgb(42, 42, 60);
                        _lastMoveButton.ForeColor = Color.White;
                        _lastMoveButton.UseVisualStyleBackColor = false;
                    }

                    // Set qu?n m?i
                    btn.Text = text.ToUpper();
                    btn.Enabled = true;
                    btn.ForeColor = Color.Black;
                    btn.Font = new Font("Segoe UI", 18, FontStyle.Bold);
                    btn.UseVisualStyleBackColor = false;

                    // Highlight ? v?a ??nh
                    btn.BackColor = Color.LightSkyBlue;

                    _lastMoveButton = btn;

                    SystemSounds.Asterisk.Play();

                    return;
                }
            }
        }
        public void HighlightWinningCells(List<WinCoordinate> winningCells)
        {
            foreach (var winCell in winningCells)
            {
                foreach (Control control in Controls)
                {
                    if (control is Button btn && btn.Tag?.ToString() == $"{winCell.X},{winCell.Y}")
                    {
                        btn.BackColor = Color.Gold;
                        btn.ForeColor = Color.Black;
                        btn.Font = new Font("Segoe UI", 18, FontStyle.Bold);
                        btn.UseVisualStyleBackColor = false;
                        btn.Enabled = true;
                    }
                }
            }
        }

        public void NewGame()
        {
            _lastMoveButton = null;
            foreach (Control control in Controls)
            {
                if (control is Button btn)
                {
                    btn.Text = "";
                    btn.Enabled = true;
                    btn.ForeColor = Color.White;
                    btn.Font = new Font("Segoe UI", 18, FontStyle.Bold);
                    btn.UseVisualStyleBackColor = false;
                    btn.BackColor = Color.FromArgb(42, 42, 60);
                }
            }
        }


        public void SetBoardEnabled(bool enabled)
        {
            _canPlay = enabled;
        }

        private void BoardControl_Load(object sender, EventArgs e)
        {

        }
        public event Action<int, int>? OnCellClicked;
    }
}