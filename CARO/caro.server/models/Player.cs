using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caro.server.Models
{
    public class Player
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public int Score { get; set; }
        public bool IsPlaying { get; set; }

        public Player(string username, string displayName)
        {
            Username = username;
            DisplayName = displayName;
            Score = 0;
            IsPlaying = false;
        }
    }
}