using System;
using System.Drawing;
using System.Windows.Forms;

namespace CaroGame.Client.UI_Components
{
    public class CaroBoard : Control
    {
        private const int Rows = 15;
        private const int Cols = 15;
        private const int CellSize = 30;
        private int[,] boardData = new int[Rows, Cols];

        public event EventHandler<Point>? CellClicked;

        public CaroBoard()
        {
            this.Size = new Size(Cols * CellSize + 1, Rows * CellSize + 1);
            this.DoubleBuffered = true; // Chống nhấp nháy khi vẽ
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pen = new Pen(Color.Gray);

            // Vẽ lưới
            for (int i = 0; i <= Rows; i++) g.DrawLine(pen, 0, i * CellSize, Cols * CellSize, i * CellSize);
            for (int j = 0; j <= Cols; j++) g.DrawLine(pen, j * CellSize, 0, j * CellSize, Rows * CellSize);

            // Vẽ quân cờ
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (boardData[r, c] == 1) DrawSymbol(g, "X", Color.Red, r, c);
                    else if (boardData[r, c] == 2) DrawSymbol(g, "O", Color.Blue, r, c);
                }
            }
        }

        private void DrawSymbol(Graphics g, string symbol, Color color, int r, int c)
        {
            Font font = new Font("Arial", 16, FontStyle.Bold);
            g.DrawString(symbol, font, new SolidBrush(color), c * CellSize + 5, r * CellSize + 2);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            int r = e.Y / CellSize;
            int c = e.X / CellSize;

            if (r >= 0 && r < Rows && c >= 0 && c < Cols && boardData[r, c] == 0)
            {
                CellClicked?.Invoke(this, new Point(r, c));
            }
        }

        public void UpdateCell(int r, int c, int player)
        {
            boardData[r, c] = player;
            this.Invalidate(); // Yêu cầu vẽ lại bàn cờ
        }
    }
}