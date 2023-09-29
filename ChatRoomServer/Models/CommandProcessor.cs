using Hangfire;

namespace ChatRoomServer.Models
{
    internal static class CommandProcessor
    {
        internal static async Task<bool> ProcessCommand(string command)
        {
            switch (command)
            {
                case "SAVE_MESSAGES": return SaveMessages();
                default: break;
            }
            return false;
        }

        private static bool SaveMessages()
        {
            try
            {
                BackgroundJob.Enqueue<MessageStorageManager>(call => call.SaveMessagesToDatabaseAsync());
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
