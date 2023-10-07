using Hangfire;
using System.Net.WebSockets;

namespace ChatRoomServer.Models
{
    internal static class CommandProcessor
    {
        internal static async Task<Tuple<bool, string>> ProcessCommand(string command, WebSocket webSocket, HttpContext httpContext)
        {
            string[] args = command.Split(' ');
            try
            {
                switch (args[0])
                {
                    case "SAVE_MESSAGES": return SaveMessages();
                    case "LOAD": return LoadMessageCount(args[1], webSocket);
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
    }
}
