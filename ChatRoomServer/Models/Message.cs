namespace ChatRoomServer.Models
{
    public class Message
    {
        public string Id { get; set; }
        public string? Body { get; set; }
        public string Sender { get; set; }
        public DateTime TimeSent { get; set; }
    }
}
