using System.Security.Cryptography;
using System.Text;

namespace HolisticApp.Helpers
{
    public static class HashHelper
    {
        public static string ComputeHash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hashBytes);
        }
    }
}