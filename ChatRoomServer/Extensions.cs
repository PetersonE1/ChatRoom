using System.Security.Cryptography;
using System.Text;

namespace ChatRoomServer
{
    public static class Extensions
    {
        public static string GetSecureHash(this string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            using (HashAlgorithm sha = SHA256.Create())
            {
                byte[] result = sha.ComputeHash(bytes);
                StringBuilder builder = new StringBuilder();

                foreach (byte b in result)
                    builder.AppendFormat("{0:X2}", b);

                return builder.ToString();
            }
        }
    }
}
