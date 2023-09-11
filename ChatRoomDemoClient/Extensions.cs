using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoomDemoClient
{
    internal static class Extensions
    {
        internal static void WriteRequestToConsole(this HttpResponseMessage response)
        {
            if (response is null)
            {
                return;
            }

            var request = response.RequestMessage;
            Console.Write($"{request?.Method} ");
            Console.Write($"{request?.RequestUri} ");
            Console.WriteLine($"HTTP/{request?.Version}");
        }
    }

    internal class WebSocketMessage
    {
        public ArraySegment<byte> Bytes;
        public WebSocketMessageType Type;
        public bool EndOfMessage;
        public CancellationToken CancellationToken;

        public WebSocketMessage(string s, WebSocketMessageType type)
        {
            Bytes = new ArraySegment<byte>(Encoding.UTF8.GetBytes(s), 0, s.Length);
            Type = type;
            EndOfMessage = true;
            CancellationToken = CancellationToken.None;
        }
    }
}
