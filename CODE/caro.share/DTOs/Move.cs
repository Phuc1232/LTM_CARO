using System;
using System.Collections.Generic;
using System.Text;

namespace caro.share.DTOs
{
    public class MoveRequestDTO
    {
        public int row { get; set; }
        public int col { get; set; }
    }

    public class MoveNotifyDTO
    {
        public string player { get; set; }
        public int row { get; set; }
        public int col { get; set; }
        public string nextTurn { get; set; }
    }
    public class GameEndNotifyDTO
    {
        public string WinnerName { get; set; }

        public string reason { get; set; }
        public List<WinCoordinate> WinningCells { get; set; } = new List<WinCoordinate>();

    }
    public class WinCoordinate
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
