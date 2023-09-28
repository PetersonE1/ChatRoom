using Microsoft.EntityFrameworkCore;

namespace ChatRoomServer.Models
{
    public class MessageContextPersistent : DbContext
    {
        public MessageContextPersistent(DbContextOptions<MessageContextPersistent> options) : base(options) { }

        public DbSet<Message> Messages { get; set; } = null!;
    }
}
