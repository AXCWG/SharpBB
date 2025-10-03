using System.Security.Cryptography;
using System.Text;

namespace SharpBB.Server;

public static class StringExt
{
    extension(string input)
    {
        public string Sha256HexHashString()
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            var hex = BitConverter.ToString(hash).Replace("-", "").ToLower();
            return hex;
        }
    }
    
}