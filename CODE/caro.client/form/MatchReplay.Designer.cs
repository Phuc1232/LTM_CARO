namespace caro.client.form
{
    partial class MatchReplay
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }
        private List<Point> FindWinningCells()
        {
            string[,] board = new string[15, 15];

            for (int i = 0; i < _moves.Count; i++)
            {
                var move = _moves[i];

                if (move.Row < 0 || move.Row >= 15 || move.Col < 0 || move.Col >= 15)
                    continue;

                board[move.Row, move.Col] = i % 2 == 0 ? "X" : "O";
            }

            int[,] directions =
            {
        { 0, 1 },
        { 1, 0 },
        { 1, 1 },
        { 1, -1 }
    };

            for (int row = 0; row < 15; row++)
            {
                for (int col = 0; col < 15; col++)
                {
                    if (string.IsNullOrEmpty(board[row, col]))
                        continue;

                    string symbol = board[row, col];

                    for (int d = 0; d < 4; d++)
                    {
                        List<Point> cells = new List<Point>();

                        for (int k = 0; k < 5; k++)
                        {
                            int r = row + directions[d, 0] * k;
                            int c = col + directions[d, 1] * k;

                            if (r < 0 || r >= 15 || c < 0 || c >= 15)
                                break;

                            if (board[r, c] != symbol)
                                break;

                            cells.Add(new Point(r, c));
                        }

                        if (cells.Count == 5)
                            return cells;
                    }
                }
            }

            return new List<Point>();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // MatchReplay
            // 
            ClientSize = new Size(284, 261);
            ForeColor = Color.White;
            Name = "MatchReplay";
            Load += MatchReplay_Load;
            ResumeLayout(false);
        }
        private void MatchReplay_Load(object sender, EventArgs e)
        {

        }
    }
}