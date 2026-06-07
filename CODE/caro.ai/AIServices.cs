using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace caro.ai
{
    public class AIServices
    {
        public const int SIZE = 15;

        public const int EMPTY = 0;
        private const int CENTER = 7;

        private static readonly int[,] PositionalMatrix = InitializePositionalMatrix();
        private static int[,] InitializePositionalMatrix()
        {
            int[,] matrix = new int[SIZE, SIZE];
            for (int r = 0; r < SIZE; r++)
            {
                for (int c = 0; c < SIZE; c++)
                {
                    matrix[r, c] = Math.Max(0, CENTER - Math.Max(Math.Abs(r - CENTER), Math.Abs(c - CENTER)));
                }
            }
            return matrix;

        }
        private readonly int[,] _board = new int[SIZE, SIZE];

        public int[,] GetBoardState()
        {
            return (int[,])_board.Clone();
        }

        private static bool InBounds(int r, int c)
        {
            return r >= 0 && r < SIZE && c >= 0 && c < SIZE;
        }

        public static List<(int r, int c)> GetVaildMove(int[,] boardstate)
        {
            var moves = new HashSet<(int r, int c)>();
            bool hasPieces = false;

            for (int r = 0; r < SIZE; r++)
            {
                for (int c = 0; c < SIZE; c++)
                {
                    if (boardstate[r, c] != EMPTY)
                    {
                        hasPieces = true;
                        for (int dr = -2; dr <= 2; dr++)
                        {
                            for (int dc = -2; dc <= 2; dc++)
                            {
                                int nr = r + dr;
                                int nc = c + dc;
                                if (boardstate[nr, nc] == EMPTY)
                                {
                                    moves.Add((nr, nc));
                                }
                            }
                        }
                    }
                }
            }
            if (!hasPieces)
            {
                return new List<(int r, int c)> { (CENTER, CENTER) };

            }
            return moves.ToList();
        }
    }
}
