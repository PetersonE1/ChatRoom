using ChatRoomDemoClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatRoomClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HttpClient _client;
        WebSocket? _webSocket;

        public MainWindow()
        {
            _client = new()
            {
                BaseAddress = new Uri("https://localhost:7185")
            };
            InitializeComponent();
            ChatScreen.Visibility = Visibility.Collapsed;
        }

        private async void B_Login_Click(object sender, RoutedEventArgs e)
        {
            string userpass = I_Username.Text + ":" + I_Password.Password;
            string userHash = userpass.ToBase64();
            await AuthenticationHandler.Authenticate(userHash, _client);
            LoginScreen.IsEnabled = false;
            LoginScreen.Visibility = Visibility.Collapsed;
            ChatScreen.IsEnabled = true;
            ChatScreen.Visibility = Visibility.Visible;
        }

        private async void B_Register_Click(object sender, RoutedEventArgs e)
        {
            string userpass = I_Username.Text + ":" + I_Password.Password;
            string userHash = userpass.ToBase64();
            await AuthenticationHandler.Register(userHash, _client);
            await AuthenticationHandler.Authenticate(userHash, _client);
            LoginScreen.IsEnabled = false;
            LoginScreen.Visibility = Visibility.Collapsed;
            ChatScreen.IsEnabled = true;
            ChatScreen.Visibility = Visibility.Visible;
        }

        private async void B_RefreshLogin_Click(object sender, RoutedEventArgs e)
        {
            string userpass = I_Username.Text + ":" + I_Password.Password;
            string userHash = userpass.ToBase64();
            await AuthenticationHandler.Authenticate(userHash, _client);
        }

        private async void B_Disconnect_Click(object sender, RoutedEventArgs e)
        {
            if (_webSocket != null)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", default);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_webSocket == null)
                return;

            string s = I_Command.Text.ToBase64();

            await _webSocket.SendAsync(
                            new ArraySegment<byte>(Encoding.UTF8.GetBytes(s), 0, s.Length),
                            WebSocketMessageType.Binary,
                            true,
                            CancellationToken.None);

            var bytes = new byte[1024];
            var result = await _webSocket.ReceiveAsync(bytes, default);
            string res = Encoding.UTF8.GetString(bytes, 0, result.Count);
        }
    }
}
