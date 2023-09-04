using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoomDemoClient
{
    internal static class AuthenticationHandler
    {
        public static void GetCredentials(out string? username, out string? password)
        {
            Console.Write("Enter username: ");
            username = Console.ReadLine();
            Console.Write("Enter password: ");
            password = Console.ReadLine();
        }

        public static void Authenticate(string username, string password)
        {

        }
    }
}
