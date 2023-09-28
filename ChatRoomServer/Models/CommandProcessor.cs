using ChatRoomServer.Controllers;

namespace ChatRoomServer.Models
{
    internal static class CommandProcessor
    {
        public static MessageStorageController? _messageController;

        internal static async Task<bool> ProcessCommand(string command)
        {
            switch (command)
            {
                case "SAVE_MESSAGES": return await SaveMessages();
                default: break;
            }
            return false;
        }

        private static async Task<bool> SaveMessages()
        {
            if (_messageController == null)
                return false;
            try
            {
                await _messageController.SaveMessagesToDatabaseAsync(null);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
