using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public void UpdateBoard(int row, int col, int player)
        {
            if (row >= 0 && row < SIZE && col >= 0 && col < SIZE)
            {
                _board[row, col] = player;
            }
        }
        public void ResetBoard()
        {
            Array.Clear(_board, 0, _board.Length);
        }
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
                                if (InBounds(nr, nc) && boardstate[nr, nc] == EMPTY)
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
        public static GameStatus CheckWin(int[,] boardstate)
        {
            int[][] directions = new int[][]
            {
                new[] {0, 1},  // Ngang
                new[] {1, 0},  // Dọc
                new[] {1, 1},  // Chéo chính
                new[] {1, -1}  // Chéo phụ
            };

            for (int r = 0; r < SIZE; r++)
            {
                for (int c = 0; c < SIZE; c++)
                {
                    int player = boardstate[r, c];
                    if (player == EMPTY) continue;

                    foreach (var dir in directions)
                    {
                        int dr = dir[0];
                        int dc = dir[1];
                        int count = 1;
                        var WinningCells = new List<(int r, int c)> { (r, c) };

                        int currR = r + dr;
                        int currC = c + dc;

                        while (InBounds(currR, currC) && boardstate[currR, currC] == player)
                        {
                            count++;
                            WinningCells.Add((currR, currC));
                            currR += dr;
                            currC += dc;
                        }
                        currR = r - dr;
                        currC = c - dc;
                        while (InBounds(currR, currC) && boardstate[currR, currC] == player)
                        {
                            count++;
                            WinningCells.Add((currR, currC));
                            currR -= dr;
                            currC -= dc;
                        }
                        if (count >= 5)
                        {
                            return new GameStatus { Winner = player, IsDraw = false, WinningCells = WinningCells };
                        }
                    }
                }
            }
            bool isFull = true;
            for (int r = 0; r < SIZE; r++)
            {
                for (int c = 0; c < SIZE; c++)
                {
                    if (boardstate[r, c] == EMPTY)
                    {
                        isFull = false;
                        break;
                    }
                }
                if (!isFull) break;
            }
            return new GameStatus { Winner = null, IsDraw = isFull, WinningCells = new List<(int r, int c)>() };
        }
        private static int ScorePatternFromCell(int[,] boardstate, int row, int col, int dr, int dc, int player)
        {
            if (!InBounds(row, col) || boardstate[row, col] != player) return 0;

            int prevR = row - dr;
            int prevC = col - dc;

            if (InBounds(prevR, prevC) && boardstate[prevR, prevC] == player) return 0;

            int length = 0;
            int r = row;
            int c = col;
            while (InBounds(r, c) && boardstate[r, c] == player)
            {
                length++;
                r += dr;
                c += dc;
            }
            int endR1 = row - dr;
            int endC1 = col - dc;
            int endR2 = r;
            int endC2 = c;
            int openEnds = 0;
            if (InBounds(endR1, endC1) && boardstate[endR1, endC1] == EMPTY) openEnds++;
            if (InBounds(endR2, endC2) && boardstate[endR2, endC2] == EMPTY) openEnds++;
            if (length >= 5) return 100000;
            if (length == 4)
            {
                if (openEnds == 2) return 10000;
                if (openEnds == 1) return 5000;
            }
            if (length == 3)
            {
                if (openEnds == 2) return 1000;
                if (openEnds == 1) return 200;
            }
            if (length == 2)
            {
                if (openEnds == 2) return 50;
            }
            return 0;
        }
        private static int EvaluateSinglePlayer(int[,] boardState, int player)
        {
            int total = 0;
            int[][] directions = new int[][]
            {
                new[] {0, 1},
                new[] {1, 0},
                new[] {1, 1},
                new[] {1, -1}
            };
            for (int r = 0; r < SIZE; r++)
            {
                for (int c = 0; c < SIZE; c++)
                {
                    foreach (var dir in directions)
                    {
                        total += ScorePatternFromCell(boardState, r, c, dir[0], dir[1], player);
                    }
                }
            }
            return total;
        }
        public static int EvaluateBoard(int[,] boardState, int aiPlayerId)
        {
            int opponentId = aiPlayerId == 1 ? 2 : 1;
            int bonusAI = 0;
            int bonusOpp = 0;
            for (int r = 0; r < SIZE; r++)
            {
                for (int c = 0; c < SIZE; c++)
                {
                    if (boardState[r, c] == aiPlayerId) bonusAI += PositionalMatrix[r, c];
                    else if (boardState[r, c] == opponentId) bonusOpp += PositionalMatrix[r, c];
                }
            }
            int scoreAI = EvaluateSinglePlayer(boardState, aiPlayerId);
            int scoreOpp = EvaluateSinglePlayer(boardState, opponentId);
            return (scoreAI - scoreOpp) + (bonusAI - bonusOpp);
        }
        public static (int score, (int r, int c)? move) MiniMax(int[,] boardstate, int depthLimit,
            int alpha, int beta, bool isMaximizing, int aiId)
        {
            var status = CheckWin(boardstate);
            if (status.Winner != null || status.IsDraw || depthLimit == 0)
            {
                int score = EvaluateBoard(boardstate, aiId);
                if (status.Winner == aiId) score += 1000000 + depthLimit * 1000;
                else if (status.Winner != null) score -= 1000000 + depthLimit * 1000;
                return (score, null);
            }

            var validMoves = GetVaildMove(boardstate);

            if (validMoves.Count == 0) return (EvaluateBoard(boardstate, aiId), null);

            int currPlayer = isMaximizing ? aiId : (aiId == 1 ? 2 : 1);

            if (depthLimit >= 2)
            {
                var scoreMoves = new List<ScoredMove>();

                foreach (var move in validMoves)
                {
                    boardstate[move.r, move.c] = currPlayer;
                    int s = EvaluateBoard(boardstate, currPlayer);
                    boardstate[move.r, move.c] = EMPTY;
                    scoreMoves.Add(new ScoredMove { Move = move, Score = isMaximizing ? s : -s });
                }
                scoreMoves.Sort((a, b) => b.Score.CompareTo(a.Score));
                validMoves = scoreMoves.Select(m => m.Move).ToList();
            }

            (int r, int c)? bestMove = null;

            if (isMaximizing)
            {
                int bestScore = int.MinValue;

                foreach (var move in validMoves)
                {
                    boardstate[move.r, move.c] = currPlayer;
                    var result = MiniMax(boardstate, depthLimit - 1, int.MaxValue, int.MinValue, false, aiId);
                    boardstate[move.r, move.c] = EMPTY;

                    if (result.score > bestScore)
                    {
                        bestScore = result.score;
                        bestMove = result.move;
                    }
                    alpha = Math.Max(alpha, beta);
                    if (alpha >= beta) break;
                }
                return (bestScore, bestMove);
            }
            else
            {
                int bestScore = int.MaxValue;
                foreach (var move in validMoves)
                {
                    boardstate[move.r, move.c] = currPlayer;
                    var result = MiniMax(boardstate, depthLimit - 1, alpha, beta, true, aiId);
                    boardstate[move.r, move.c] = EMPTY;
                    if (result.score < bestScore)
                    {
                        bestScore = result.score;
                        bestMove = move;
                    }
                    beta = Math.Min(beta, bestScore);
                    if (beta <= alpha) break; // Cắt tỉa
                }
                return (bestScore, bestMove);
            }
        }
        public class GameStatus
        {
            public int? Winner { get; set; }
            public bool IsDraw { get; set; }
            public List<(int r, int c)> WinningCells { get; set; } = new();
        }
        private class ScoredMove
        {
            public (int r, int c) Move { get; set; }
            public int Score { get; set; }
        }
    }
    
}
