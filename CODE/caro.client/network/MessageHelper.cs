public static class MessageHelper 
{
    // Chuyển object thành chuỗi định dạng riêng (ví dụ: "Move|10|20")
    public static string Serialize(BaseMessage msg) 
    {
        if (msg is MoveMessage m) return $"Move|{m.X}|{m.Y}";
        if (msg is ChatMessage c) return $"Chat|{c.Message}";
        return "";
    }

    // Đọc chuỗi và tách ra thành đối tượng
    public static BaseMessage Deserialize(string data) 
    {
        string[] parts = data.Split('|');
        string type = parts[0];

        if (type == "Move") 
        {
            return new MoveMessage { Type = "Move", X = int.Parse(parts[1]), Y = int.Parse(parts[2]) };
        }
        else if (type == "Chat") 
        {
            return new ChatMessage { Type = "Chat", Message = parts[1] };
        }
        return null;
    }
}