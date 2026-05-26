using System.Text.Json;

namespace CaroGame.Client.Network
{
    public static class MessageHelper
    {
        public static string Serialize<T>(T message)
        {
            return JsonSerializer.Serialize(message);
        }

        public static object? Deserialize(string json)
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
    }
}