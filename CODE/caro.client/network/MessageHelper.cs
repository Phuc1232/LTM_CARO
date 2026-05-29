using System.Text.Json;

namespace CaroGame.Client.Network
{
    public static class MessageHelper
    {
        public static string Serialize(object obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public static BaseMessage? Deserialize(string json)
        {
            try
            {
                using JsonDocument doc =
                    JsonDocument.Parse(json);

                string? type =
                    doc.RootElement
                        .GetProperty("Type")
                        .GetString();

                switch (type)
                {
                    case "MOVE":
                        return JsonSerializer
                            .Deserialize<MoveMessage>(json);

                    case "CHAT":
                        return JsonSerializer
                            .Deserialize<ChatMessage>(json);

                    case "STATUS":
                        return JsonSerializer
                            .Deserialize<GameStatus>(json);

                    default:
                        return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}