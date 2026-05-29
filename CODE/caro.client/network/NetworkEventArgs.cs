namespace CaroGame.Client.Network
{
    public class MessageReceivedEventArgs
        : EventArgs
    {
        public string Message
        {
            get;
            set;
        }

        public MessageReceivedEventArgs(
            string message)
        {
            Message = message;
        }
    }
}