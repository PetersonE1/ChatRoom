﻿using Microsoft.EntityFrameworkCore;

namespace ChatRoomServer.Models
{
    public class MessageContext : DbContext
    {
        public MessageContext(DbContextOptions<MessageContext> options) : base(options) { }

        public DbSet<Message> Messages { get; set; } = null!;
    }
}
