using ChatRoomServer.Controllers;
using ChatRoomServer.Repository;
using Microsoft.AspNetCore.Server.IIS.Core;
using System.Net.WebSockets;
using System.Text;

namespace ChatRoomServer.Models
{
    public class ChatWebSocketManager
    {
        public static async Task ProcessRequest(WebSocket webSocket, HttpContext context, MessageContext messageContext)
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {
                string[]? messages = null;
                try
                {
                    messages = ProcessInput(Encoding.UTF8.GetString(buffer));
                }
                catch
                {
                    Console.WriteLine($"Bad input from {context.Connection.RemoteIpAddress} at {DateTime.UtcNow}");
                }

                if (messages != null)
                {
                    foreach (string message in messages)
                    {
                        Message new_message = new Message()
                        {
                            Body = message,
                            Sender = context.User.Identity?.Name ?? "Anonymous",
                            TimeSent = DateTime.UtcNow,
                            Id = ChatController.GenerateMessageID(32)
                        };
                        messageContext.Messages.Add(new_message);
                    }
                    await messageContext.SaveChangesAsync();
                }

                string s = string.Empty;
                foreach (Message message in messageContext.Messages)
                {
                    s += $"[{message.Sender} {message.TimeSent.ToLocalTime().ToShortTimeString()}] " + message.Body + "\r\n";
                }

                await webSocket.SendAsync(
                    new ArraySegment<byte>(Encoding.UTF8.GetBytes(s), 0, s.Length),
                    receiveResult.MessageType,
                    receiveResult.EndOfMessage,
                    CancellationToken.None);

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }

        private static string[] ProcessInput(string input)
        {
            return input.Split(':').Select(s => Encoding.UTF8.GetString(Convert.FromBase64String(s.Trim('\0')))).ToArray();
        }
    }
}
