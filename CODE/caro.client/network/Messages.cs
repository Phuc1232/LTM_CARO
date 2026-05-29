namespace CaroGame.Client.Network
{
    public class BaseMessage
    {
        public string Type { get; set; } = "";
    }

    public class MoveMessage : BaseMessage
    {
        public int X { get; set; }

        public int Y { get; set; }

        public MoveMessage()
        {
            Type = "MOVE";
        }
    }

    public class ChatMessage : BaseMessage
    {
        public string Message { get; set; } = "";

        public ChatMessage()
        {
            Type = "CHAT";
        }
    }

    public class GameStatus : BaseMessage
    {
        public string Status { get; set; } = "";

        public GameStatus()
        {
            Type = "STATUS";
        }
    }
}