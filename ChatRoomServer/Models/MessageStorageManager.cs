using ChatRoomServer.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ChatRoomServer.Models
{
    public class MessageStorageManager
    {
        private readonly MessageContext _messageContext;
        private readonly MessageContextPersistent _persistentMessageContext;

        public MessageStorageManager(ILogger<ChatController> logger, MessageContext messageContext, MessageContextPersistent messageContextPersistent)
        {
            _messageContext = messageContext;
            _persistentMessageContext = messageContextPersistent;
        }

        public async Task SaveMessagesToDatabaseAsync()
        {
            Console.WriteLine("Saving messages to persistent database");
            foreach (var message in _messageContext.Messages)
            {
                if (!_persistentMessageContext.Messages.Any(m => m.Id == message.Id))
                    _persistentMessageContext.Messages.Add(message);
            }
            await _persistentMessageContext.SaveChangesAsync();
        }

        public async Task LoadMessagesFromDatabaseAsync()
        {
            foreach (var message in _persistentMessageContext.Messages.OrderBy(key => key.TimeSent))
            {
                if (!_messageContext.Messages.Any(m => m.Id == message.Id))
                    _messageContext.Messages.Add(message);
            }
            await _messageContext.SaveChangesAsync();
        }

        public async Task RemoveMessage(string id)
        {
            Message? message = _messageContext.Messages.Find(id);
            if (message == null)
                return;
            _messageContext.Messages.Remove(message);
            if (_persistentMessageContext.Messages.Contains(message))
                _persistentMessageContext.Messages.Remove(message);

            await _messageContext.SaveChangesAsync();
            await _persistentMessageContext.SaveChangesAsync();
        }
    }
}
