using ChatRoomDemoClient;
using System.Net;
using System.Net.Http.Headers;

HttpClient _client = new()
{
    BaseAddress = new Uri("https://localhost:7185")
};

char loginChoice;
Console.WriteLine("Select an option\n1. Login\n2. Register an account");
do
{
    loginChoice = (char)Console.Read();
}
while (loginChoice != '1' && loginChoice != '2');

AuthenticationHandler.GetCredentials(out string? username, out string? password);
while (username == null || password == null)
{
    Console.WriteLine("Invalid entry, please try again");
    AuthenticationHandler.GetCredentials(out username, out password);

}

if (loginChoice == 2)
{
    using HttpResponseMessage response = await _client.GetAsync("user");

    response.EnsureSuccessStatusCode().WriteRequestToConsole();

    var responseData = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"{responseData}\n");
}