using ChatRoomDemoClient;
using System.Net;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;

HttpClient _client = new()
{
    BaseAddress = new Uri("https://localhost:7185")
};

char loginChoice;
Console.WriteLine("Select an option\n1. Login\n2. Register an account");
do
{
    loginChoice = Console.ReadKey(true).KeyChar;
}
while (loginChoice != '1' && loginChoice != '2');

AuthenticationHandler.GetCredentials(out string? username, out string? password);
while ((username == null || username == string.Empty) || (password == null || password == string.Empty))
{
    Console.WriteLine("Invalid entry, please try again");
    AuthenticationHandler.GetCredentials(out username, out password);

}

string userEncoding = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));

if (loginChoice == '2')
{
    await AuthenticationHandler.Register(userEncoding, _client);
}
await AuthenticationHandler.Authenticate(userEncoding, _client);

Console.WriteLine(AuthenticationHandler._token);

using SocketsHttpHandler handler = new();
using ClientWebSocket ws = new();
ws.Options.SetRequestHeader("Authorization", $"Bearer {AuthenticationHandler._token}");
await ws.ConnectAsync(new Uri("wss://localhost:7185/chat/EstablishConnection"), new HttpMessageInvoker(handler), default);

/*string[] toSend = { "first", "second", "third" };
string s = string.Empty;
foreach (string st in toSend)
    s += Convert.ToBase64String(Encoding.UTF8.GetBytes(st)) + ':';
s = s.Remove(s.Length - 1, 1);

await ws.SendAsync(
                    new ArraySegment<byte>(Encoding.UTF8.GetBytes(s), 0, s.Length),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);

var bytes = new byte[1024];
var result = await ws.ReceiveAsync(bytes, default);
string res = Encoding.UTF8.GetString(bytes, 0, result.Count);

await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", default);

Console.WriteLine(res);*/

Console.WriteLine("Choose an option:\n1. Send message\n2. Send server request\n3. Close connection");
string s = string.Empty;
while (s != "CLOSE")
{
    s = string.Empty;
    do
    {
        switch (Console.ReadKey(true).KeyChar)
        {
            case '1': s = "m-"; break;
            case '2': s = "r-"; break;
            case '3': s = "CLOSE"; break;
            default: break;
        }
    }
    while (s == string.Empty);

    if (s == "CLOSE")
        break;

    string message = Console.ReadLine() ?? string.Empty;
    s += Convert.ToBase64String(Encoding.UTF8.GetBytes(message));

    await ws.SendAsync(
                    new ArraySegment<byte>(Encoding.UTF8.GetBytes(s), 0, s.Length),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);

    var bytes = new byte[1024];
    var result = await ws.ReceiveAsync(bytes, default);
    string res = Encoding.UTF8.GetString(bytes, 0, result.Count);
    Console.WriteLine(res);
}

await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", default);