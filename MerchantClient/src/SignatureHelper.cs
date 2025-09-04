using System.Security.Cryptography;
using System.Text;

namespace MerchantClient.src
{
    public static class SignatureHelper
    {
        public static string GenerateSignature(string timeStamp, string externalId, string privateKey)
        {
            string input = timeStamp + externalId + privateKey;

            using var md5 = MD5.Create();
            byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sb = new StringBuilder();
            foreach (var b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
