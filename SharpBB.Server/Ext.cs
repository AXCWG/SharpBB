using System.Security.Cryptography;
using System.Text;

namespace SharpBB.Server;

public static class Ext
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
        public bool ToBoolean() => input.ToLowerInvariant() ==  "true";
    }

    extension(bool input)
    {
        public string ToStringStandard() => input ? "true" : "false"; 
    }
    extension(bool? input)
    {
        public string? ToStringStandard()
        {
            if (input.HasValue) 
                return input.Value ? "true" : "false"; 
            return null;
        }
    }

    extension(string? input)
    {
        public bool IsNullOrWhiteSpace()
        {
            return string.IsNullOrWhiteSpace(input); 
        }
        public string? NullIfEmpty() => input.IsNullOrWhiteSpace() ? null : input;
    }
}