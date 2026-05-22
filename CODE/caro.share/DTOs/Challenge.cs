using System;
using System.Collections.Generic;
using System.Text;

namespace caro.share.DTOs
{
    public class ChallengeRequestDTO
    {
        public string targetUsername { get; set; }
    }
    public class ChallengeNotifyDTO
    {
        public string fromUsername { get; set; }
        public string roomId { get; set; }
    }
    public class ChallengeResponseDTO
    {
        public string roomId { get; set; }
        public bool isAccepted { get; set; }
    }
    public class ChallengeResultDTO
    {
        public bool isAccepted { get; set; }
        public string message { get; set; }
        public string roomId { get; set; }
        public string opponentName { get; set; }
    }
    public class OnlinePlayerListDTO
    {
        public List<string> players { get; set; } = new List<string>();
    }
}
