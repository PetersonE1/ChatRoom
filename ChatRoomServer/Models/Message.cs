namespace ChatRoomServer.Models
{
    public class Message
    {
        public string? Body;
        public User Sender;
        public DateTime TimeSent;
        public string id;
    }
}
