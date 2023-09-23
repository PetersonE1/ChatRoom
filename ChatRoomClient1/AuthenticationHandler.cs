using ChatRoomClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoomDemoClient
{
    internal static class AuthenticationHandler
    {
        internal static string? _token;
        internal static string? _refreshToken;
        internal static WebSocket? _webSocket;

        public static void GetCredentials(out string? username, out string? password)
        {
            Console.Write("Enter username: ");
            username = Console.ReadLine();
            Console.Write("Enter password: ");
            password = Console.ReadLine();
        }

        public static async Task<bool> Authenticate(string userEncoding, HttpClient client)
        {
            HttpRequestMessage message = new HttpRequestMessage()
            {
                RequestUri = new Uri("https://localhost:7185/user/authenticate"),
                Method = HttpMethod.Get,
            };
            message.Headers.Add("Credentials", userEncoding);

            using HttpResponseMessage response = await client.SendAsync(message);

            response.EnsureSuccessStatusCode().WriteRequestToConsole();

            var responseData = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"{responseData}\n");
            try
            {
                Tokens token = JsonConvert.DeserializeObject<Tokens>(responseData);
                _token = token.Token;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task Register(string userEncoding, HttpClient client)
        {
            HttpRequestMessage message = new HttpRequestMessage()
            {
                RequestUri = new Uri("https://localhost:7185/user/register"),
                Method = HttpMethod.Get,
            };
            message.Headers.Add("Credentials", userEncoding);

            using HttpResponseMessage response = await client.SendAsync(message);

            response.EnsureSuccessStatusCode().WriteRequestToConsole();

            var responseData = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"{responseData}\n");
        }

        public static async Task ConnectToWebSocket()
        {
            using SocketsHttpHandler handler = new();
            using ClientWebSocket ws = new();
            ws.Options.SetRequestHeader("Authorization", $"Bearer {_token}");
            await ws.ConnectAsync(new Uri("wss://localhost:7185/chat/EstablishConnection"), new HttpMessageInvoker(handler), default);
            _webSocket = ws;
            await Task.Delay(-1);
        }

        private class Tokens
        {
            public string Token;
            public string RefreshToken;
        }
    }
}
