using System;
using System.Collections.Generic;
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


    }
}
