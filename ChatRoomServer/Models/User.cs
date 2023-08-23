namespace ChatRoomServer.Models
{
    public class User
    {
        public long Id { get; set; }
        public string? Username { get; set; }
        public int PasswordHash { get; set; }
    }
}
