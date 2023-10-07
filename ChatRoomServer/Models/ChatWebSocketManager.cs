using ChatRoomServer.Controllers;
using ChatRoomServer.Repository;
using Microsoft.AspNetCore.Server.IIS.Core;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace ChatRoomServer.Models
{
    public class ChatWebSocketManager
    {
        public static Dictionary<WebSocket, int> messagesToLoadCount = new Dictionary<WebSocket, int>();

        public static async Task ProcessRequest(WebSocket webSocket, HttpContext context, MessageContext messageContext)
        {
            messagesToLoadCount.Add(webSocket, 50);
            Console.WriteLine($"Opening connection with {context.Connection.RemoteIpAddress} at {DateTime.UtcNow}");
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), default);

            while (!receiveResult.CloseStatus.HasValue)
            {
                string[] tempInput = Encoding.UTF8.GetString(buffer).Split('$');
                string input = tempInput[0];
                DateTime cutoff = DateTime.FromBinary(long.Parse(tempInput[1].Trim('\0')));
                if (input == "NULL")
                {
                    TextMessage(null, cutoff, context, messageContext, webSocket);

                    buffer = new byte[1024 * 4];
                    receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), default);

                    continue;
                }

                string[]? messages = null;
                try
                {
                    messages = ProcessInput(input);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Bad input from {context.Connection.RemoteIpAddress} at {DateTime.UtcNow}:\n{ex.StackTrace}");
                }
                switch (receiveResult.MessageType)
                {
                    case WebSocketMessageType.Text: TextMessage(messages, cutoff, context, messageContext, webSocket); break;
                    case WebSocketMessageType.Binary: CommandMessage(messages, context, messageContext, webSocket); break;
                    case WebSocketMessageType.Close: break;
                    default: break;
                }

                if (receiveResult.MessageType == WebSocketMessageType.Close)
                    break;

                buffer = new byte[1024 * 4];
                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), default);
            }

            Console.WriteLine($"Closing connection with {context.Connection.RemoteIpAddress} at {DateTime.UtcNow} with {receiveResult.CloseStatus ?? WebSocketCloseStatus.Empty}: {receiveResult.CloseStatusDescription ?? "null"}");
            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);

            messagesToLoadCount.Remove(webSocket);
        }

        private static async void TextMessage(string[]? messages, DateTime cutoffTime, HttpContext context, MessageContext messageContext, WebSocket webSocket)
        {
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

            int messageCount = messagesToLoadCount[webSocket];
            Message[] toSend = messageContext.Messages.AsEnumerable().TakeLast(messageCount).ToArray();
            string s = JsonConvert.SerializeObject(toSend.FirstOrDefault(
                message => message.TimeSent.CompareTo(cutoffTime) > 0));

            await webSocket.SendAsync(
                new ArraySegment<byte>(Encoding.UTF8.GetBytes(s), 0, s.Length),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }

        private static async void CommandMessage(string[]? messages, HttpContext context, MessageContext messageContext, WebSocket webSocket)
        {
            List<Tuple<bool, string>> results = new List<Tuple<bool, string>>();
            if (messages != null)
                foreach (string message in messages)
                    results.Add(await CommandProcessor.ProcessCommand(message, webSocket, context));

            string s = string.Join(", ", results);

            await webSocket.SendAsync(
                new ArraySegment<byte>(Encoding.UTF8.GetBytes(s), 0, s.Length),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }

        public static void PurgeWebsocketsFromDict()
        {
            List<WebSocket> toPurge = new List<WebSocket>();
            foreach (WebSocket socket in messagesToLoadCount.Keys)
                if (socket.State != WebSocketState.Open)
                    toPurge.Add(socket);

            foreach (WebSocket socket in toPurge)
                messagesToLoadCount.Remove(socket);
        }

        private static string[] ProcessInput(string input)
        {
            return input.Split(':').Select(s => Encoding.UTF8.GetString(Convert.FromBase64String(s.Trim('\0')))).ToArray();
        }
    }
}