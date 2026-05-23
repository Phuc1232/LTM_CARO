public abstract class BaseMessage {
    public string Type { get; set; }
}


public class MoveMessage : BaseMessage {
    public int X { get; set; }
    public int Y { get; set; }
}

public class ChatMessage : BaseMessage {
    public string Content { get; set; }
}