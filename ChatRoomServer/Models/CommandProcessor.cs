using Hangfire;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ChatRoomServer.Models
{
    internal static class CommandProcessor
    {
        internal static async Task<Tuple<bool, string>> ProcessCommand(string command, WebSocket webSocket, HttpContext httpContext, IConfiguration configuration, MessageContext messages)
        {
            string[] args = command.Split(' ');
            try
            {
                switch (args[0])
                {
                    case "SAVE_MESSAGES": return SaveMessages();
                    case "LOAD": return LoadMessageCount(args[1], webSocket);
                    case "ELEVATE": return ElevateSession(args[1], httpContext, configuration);
                    case "GET_ID": return GetMessageID(args[1], messages);
                    case "DELETE": return await DeleteMessage(args[1], httpContext, messages);
                    default: break;
                }
            }
            catch (IndexOutOfRangeException)
            {
                return new Tuple<bool, string>(false, "Missing arguments");
            }
            return new Tuple<bool, string>(false, "Invalid command");
        }

        private static Tuple<bool, string> SaveMessages()
        {
            try
            {
                BackgroundJob.Enqueue<MessageStorageManager>(call => call.SaveMessagesToDatabaseAsync());
                return new Tuple<bool, string>(true, "Messages saved");
            }
            catch
            {
                return new Tuple<bool, string>(false, "Failed to enqueue command");
            }
        }

        private static Tuple<bool, string> LoadMessageCount(string rawCount, WebSocket socket)
        {
            if (!int.TryParse(rawCount, out int count))
                return new Tuple<bool, string>(false, "Unable to parse input");

            ChatWebSocketManager.messagesToLoadCount[socket] = count;
            return new Tuple<bool, string>(true, $"Loading latest {count} messages");
        }

        private static Tuple<bool, string> ElevateSession(string key, HttpContext context, IConfiguration configuration)
        {   
            if (key != configuration["ElevationKey"]) return new Tuple<bool, string>(false, "Invalid key");
            context.User.AddIdentity(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Chat-Admin")
            }));
            return new Tuple<bool, string>(true, "Session elevated");
        }

        private static Tuple<bool, string> GetMessageID(string raw_selector, MessageContext messages)
        {
            if (!int.TryParse(raw_selector, out int selector))
                return new Tuple<bool, string>(false, "Unable to parse selector");
            selector++;
            try
            {
                string id = messages.Messages.AsEnumerable().ElementAt(messages.Messages.Count() - selector).Id;
                return new Tuple<bool, string>(true, id);
            }
            catch (ArgumentOutOfRangeException)
            {
                return new Tuple<bool, string>(false, "Selector out of range");
            }
        }

        private static async Task<Tuple<bool, string>> DeleteMessage(string id, HttpContext context, MessageContext messages)
        {
            if (context.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role && claim.Value == "Chat-Admin") == default)
                return new Tuple<bool, string>(false, "Unauthorized");
            BackgroundJob.Enqueue<MessageStorageManager>(call => call.RemoveMessage(id));
            Message deletionCommand = new Message()
            {
                Sender = "SERVER",
                Id = "REMOVE",
                Body = id,
                TimeSent = DateTime.UtcNow
            };
            foreach (WebSocket socket in ChatWebSocketManager.messagesToLoadCount.Keys)
            {
                string s = JsonConvert.SerializeObject(deletionCommand);

                await socket.SendAsync(
                    new ArraySegment<byte>(Encoding.UTF8.GetBytes(s), 0, s.Length),
                    WebSocketMessageType.Binary,
                    false,
                    CancellationToken.None);
            }
            return new Tuple<bool, string>(true, "Removing message");
        }
    }
}
