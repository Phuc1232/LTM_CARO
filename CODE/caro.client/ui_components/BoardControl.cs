using System;
using System.Drawing;
using System.Windows.Forms;

namespace caro.client.ui_components
{
    public partial class BoardControl : UserControl
    {
        private const int ROWS = 15;
        private const int COLS = 15;
        private const int CELL_SIZE = 40;

        public event Action<int, int>? OnCellClicked;

        public BoardControl()
        {
            InitializeComponent();
            CreateBoard();

            Width = COLS * CELL_SIZE;
            Height = ROWS * CELL_SIZE;
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
                    cell.Tag = $"{row},{col}";

                    cell.FlatStyle = FlatStyle.Flat;
                    cell.FlatAppearance.BorderSize = 1;
                    cell.FlatAppearance.BorderColor = Color.FromArgb(90, 90, 120);

                    cell.BackColor = Color.FromArgb(42, 42, 60);
                    cell.ForeColor = Color.White;
                    cell.Font = new Font("Segoe UI", 14, FontStyle.Bold);

                    cell.Margin = new Padding(0);
                    cell.Padding = new Padding(0);

                    cell.Click += Cell_Click;

                    Controls.Add(cell);
                }
            }
        }

        private void Cell_Click(object? sender, EventArgs e)
        {
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
                    btn.Text = text;
                    btn.Enabled = false;
                    return;
                }
            }
        }

        public void SetBoardEnabled(bool enabled)
        {
            foreach (Control control in Controls)
            {
                if (control is Button btn)
                {
                    btn.Enabled = enabled && string.IsNullOrEmpty(btn.Text);
                }
            }
        }

        public void NewGame()
        {
            foreach (Control control in Controls)
            {
                if (control is Button btn)
                {
                    btn.Text = "";
                    btn.Enabled = true;
                }
            }
        }

        private void BoardControl_Load(object sender, EventArgs e)
        {
        }
    }
}