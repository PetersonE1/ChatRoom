using System.Net.WebSockets;

namespace ChatRoomServer.Repository
{
    public interface IWebSocketManager
    {
        abstract static Task ProcessRequest(WebSocket webSocket, string user);
    }
}
