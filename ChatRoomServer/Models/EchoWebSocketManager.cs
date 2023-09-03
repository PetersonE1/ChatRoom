using ChatRoomServer.Repository;
using System.Net.WebSockets;
using System.Text;

namespace ChatRoomServer.Models
{
    public class EchoWebSocketManager : IWebSocketManager
    {
        public static async Task ProcessRequest(WebSocket webSocket, string user)
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(
                    new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                    receiveResult.MessageType,
                    receiveResult.EndOfMessage,
                    CancellationToken.None);
                Console.WriteLine($"[{user}] " + Encoding.UTF8.GetString(buffer));

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            Console.WriteLine($"Connection closed with {user}");
            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }
    }
}
