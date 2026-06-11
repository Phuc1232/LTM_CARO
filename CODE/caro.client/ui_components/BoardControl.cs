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
    public partial class BoardControl : UserControl
    {
        private const int ROWS = 15;
        private const int COLS = 15;
        private const int CELL_SIZE = 40;
        private bool isXTurn = true;
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

            isXTurn = true;
        }

        private void BoardControl_Load(object sender, EventArgs e)
        {

        }
        public event Action<int, int>? OnCellClicked;
    }
}