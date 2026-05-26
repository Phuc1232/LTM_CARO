namespace CaroGame.Client.Network
{
    public class MoveMessage
    {
        public string Type { get; set; } = "MOVE";

        public int X { get; set; }

        public int Y { get; set; }
    }

    public class ChatMessage
    {
        public string Type { get; set; } = "CHAT";

        public string Message { get; set; } = "";
    }

    public class GameStatus
    {
        public string Type { get; set; } = "STATUS";

        public string Status { get; set; } = "";
    }
}