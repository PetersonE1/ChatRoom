/*using ChatRoomDemoClient;
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

/*WebSocketMessageType? messageType = null;
do
{
    Console.WriteLine("Choose an option:\n1. Send message\n2. Send server request\n3. Close connection");
    messageType = null;
    string prefix = string.Empty;
    do
    {
        switch (Console.ReadKey(true).KeyChar)
        {
            case '1': messageType = WebSocketMessageType.Text; prefix = "message"; break;
            case '2': messageType = WebSocketMessageType.Binary; prefix = "command"; break;
            case '3': messageType = WebSocketMessageType.Close; break;
            default: break;
        }
    }
    while (messageType == null);

    if (messageType == WebSocketMessageType.Close)
        break;

    Console.Write($"{prefix}> ");
    string message = Console.ReadLine() ?? string.Empty;
    string s = Convert.ToBase64String(Encoding.UTF8.GetBytes(message));

    await ws.SendAsync(
                    new ArraySegment<byte>(Encoding.UTF8.GetBytes(s), 0, s.Length),
                    (WebSocketMessageType)messageType,
                    true,
                    CancellationToken.None);

    var bytes = new byte[1024];
    var result = await ws.ReceiveAsync(bytes, default);
    string res = Encoding.UTF8.GetString(bytes, 0, result.Count);
    Console.WriteLine(res);
}
while (messageType != WebSocketMessageType.Close);

await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", default);
Console.WriteLine("\nConnection terminated");*/