using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace caro.server.Models
{
    public class Room
    {
        public string RoomId { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public bool IsFull => Player1 != null && Player2 != null;

        public Room(string roomId)
        {
            RoomId = roomId;
        }
    }
}
