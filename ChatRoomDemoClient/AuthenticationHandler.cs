using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoomDemoClient
{
    internal static class AuthenticationHandler
    {
        internal static string? _token;
        internal static string? _refreshToken;

        public static void GetCredentials(out string? username, out string? password)
        {
            Console.Write("Enter username: ");
            username = Console.ReadLine();
            Console.Write("Enter password: ");
            password = Console.ReadLine();
        }

        public static async Task Authenticate(string userEncoding, HttpClient client)
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
            Tokens token = JsonConvert.DeserializeObject<Tokens>(responseData);
            _token = token.Token;
            Console.WriteLine($"{responseData}\n");
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

        private class Tokens
        {
            public string Token;
            public string RefreshToken;
        }
    }
}
