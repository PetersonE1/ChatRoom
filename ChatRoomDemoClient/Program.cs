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

List<WebSocketMessage> buffer = new List<WebSocketMessage>();
do
{
    Task messageTask = ConnectionHandler.GetMessageAsync(buffer);
    Task webSocketTask = ConnectionHandler.FeedSocketBufferAsync(ws, buffer);

    await messageTask;
    await webSocketTask;
    await Task.Delay(200);
}
while (ws.State != WebSocketState.CloseSent);

await ConnectionHandler.WaitUntilSocketClosed(ws);
Console.WriteLine("\nConnection terminated");