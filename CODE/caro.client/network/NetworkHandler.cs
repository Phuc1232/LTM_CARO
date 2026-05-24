using System;
using System.Windows.Forms;

public class NetworkHandler 
{
    public void OnDataReceived(string rawData) 
    {
        try 
        {
            BaseMessage msg = MessageHelper.Deserialize(rawData);

            if (msg == null) return;

            if (msg is MoveMessage move)
            {
                Console.WriteLine($"Di chuyển tới: {move.X}, {move.Y}");
            }
            else if (msg is ChatMessage chat)
            {
                Console.WriteLine($"Tin nhắn mới: {chat.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi xử lý dữ liệu: " + ex.Message);
        }
    }
    public void SafeUpdateUI(Control control, Action updateAction) 
    {
        if (control.InvokeRequired) 
        {
            control.Invoke(updateAction);
        } 
        else 
        {
            updateAction();
        }
    }
}