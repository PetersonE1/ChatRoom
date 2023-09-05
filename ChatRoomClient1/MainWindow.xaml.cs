using ChatRoomDemoClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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

        public MainWindow()
        {
            _client = new()
            {
                BaseAddress = new Uri("https://localhost:7185")
            };
            InitializeComponent();
        }

        private async void B_Login_Click(object sender, RoutedEventArgs e)
        {
            string userpass = I_Username.Text + ":" + I_Password.Password;
            string userHash = userpass.ToBase64();
            await AuthenticationHandler.Authenticate(userHash, _client);
        }

        private async void B_Register_Click(object sender, RoutedEventArgs e)
        {
            string userpass = I_Username.Text + ":" + I_Password.Password;
            string userHash = userpass.ToBase64();
            await AuthenticationHandler.Register(userHash, _client);
            await AuthenticationHandler.Authenticate(userHash, _client);
        }
    }
}
