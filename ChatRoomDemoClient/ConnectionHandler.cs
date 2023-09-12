using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoomDemoClient
{
    internal static class ConnectionHandler
    {
        public static Task<string> GetInputAsyncTask(string prefix)
        {
            Console.Write($"{prefix}> ");
            return Task.Run(() => Console.ReadLine() ?? string.Empty);
        }

        public static Task<char> GetKeyAsyncTask()
        {
            return Task.Run(() => { return Console.ReadKey(true).KeyChar; } );
        }

        public static Task GetMessageAsync(List<WebSocketMessage> buffer)
        {
            return Task.Run(async () =>
            {
                WebSocketMessageType? messageType = null;
                Console.WriteLine("Choose an option:\n1. Send message\n2. Send server request\n3. Close connection");
                string prefix = string.Empty;
                do
                {
                    switch (await GetKeyAsyncTask())
                    {
                        case '1': messageType = WebSocketMessageType.Text; prefix = "message"; break;
                        case '2': messageType = WebSocketMessageType.Binary; prefix = "command"; break;
                        case '3': messageType = WebSocketMessageType.Close; break;
                        default: break;
                    }
                }
                while (messageType == null);

                string messageString = await GetInputAsyncTask(prefix);

                WebSocketMessage message = new WebSocketMessage(messageString, (WebSocketMessageType)messageType);
                buffer.Add(message);
                return Task.CompletedTask;
            });
        }

        public static void FeedSocketBufferAsync(WebSocket ws, List<WebSocketMessage> buffer)
        {
            List<Task> tasks = new List<Task>();
            while (buffer.Count > 0)
            {
                WebSocketMessage message = buffer.First();
                if (message.Type == WebSocketMessageType.Close)
                {
                    tasks.Add(ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", default));
                    Console.WriteLine("Connection closing...");
                    break;
                }
                tasks.Add(RunSocketLoopAsync(ws, message));
                buffer.RemoveAt(0);
            }

            Task.WaitAll(tasks.ToArray());
        }

        public static Task RunSocketLoopAsync(WebSocket ws, WebSocketMessage message)
        {
            return Task.Run(async () =>
            {
                await ws.SendAsync(message.Bytes, message.Type, message.EndOfMessage, message.CancellationToken);

                var bytes = new byte[1024];
                var result = await ws.ReceiveAsync(bytes, default);
                string res = Encoding.UTF8.GetString(bytes, 0, result.Count);

                Console.Clear();
                Console.WriteLine(res);
            });
        }

        public static async Task<Task> WaitUntilSocketClosed(WebSocket ws)
        {
            while (ws.State != WebSocketState.Closed)
            {
                await Task.Delay(1000);
            }
            return Task.CompletedTask;
        }
    }
}
