using ChatRoomServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChatRoomServer.Controllers
{
    public class MessageStorageController : Controller
    {
        private readonly ILogger<ChatController> _logger;
        private readonly MessageContext _messageContext;
        private readonly MessageContextPersistent _persistentMessageContext;
        private readonly Timer SaveTimer;
        private const int SAVE_DELAY = 5 * 60000;

        public MessageStorageController(ILogger<ChatController> logger, MessageContext messageContext, MessageContextPersistent messageContextPersistent)
        {
            _logger = logger;
            _messageContext = messageContext;
            _persistentMessageContext = messageContextPersistent;

            LoadMessagesFromDatabaseAsync().RunSynchronously();
            SaveTimer = new(async (o) => await SaveMessagesToDatabaseAsync(o), null, SAVE_DELAY, SAVE_DELAY);

            CommandProcessor._messageController = this;
        }

        [NonAction]
        public async Task SaveMessagesToDatabaseAsync(object? state)
        {
            foreach (var message in _messageContext.Messages)
            {
                if (_persistentMessageContext.Messages.Any(m => m.Id == message.Id))
                    _persistentMessageContext.Messages.Add(message);
            }
            await _persistentMessageContext.SaveChangesAsync();
        }

        [NonAction]
        public async Task LoadMessagesFromDatabaseAsync()
        {
            foreach (var message in _persistentMessageContext.Messages)
            {
                if (_messageContext.Messages.Any(m => m.Id == message.Id))
                    _messageContext.Messages.Add(message);
            }
            await _messageContext.SaveChangesAsync();
        }
    }
}
