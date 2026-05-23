using caro.server.network;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace caro.server.models
{
    public class GameRoom
    {
        public string RoomID { get; set; }
        public ClientHandle player1 { get; set; }
        public ClientHandle player2 { get; set; }
        public int TimeSecondPerPlayer { get; set; } //thoi gian goc cua moi player
        public int RemainingTimeP1 { get; set; }
        public int RemainingTimeP2 { get; set; }
        public string CurrentTurn { get; set; }
        public CancellationTokenSource cts { get; set; } // quan ly thoi gian ket thuc
        public bool IsGameActive { get; set; }

    }
}
