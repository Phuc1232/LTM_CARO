namespace CaroGame.Client.Network
{
    public static class NetworkLogger
    {
        public static void Log(string message)
        {
            Console.WriteLine(
                $"[NETWORK] {message}");
        }

        public static void Error(string message)
        {
            Console.WriteLine(
                $"[ERROR] {message}");
        }
    }
}