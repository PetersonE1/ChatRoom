using System.Net.WebSockets;

namespace ChatRoomServer.Repository
{
    public interface IWebsocketManager
    {
        abstract static Task Echo(WebSocket webSocket, string user);
    }
}
