using System;
using System.Net.Http;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MerchantClient.src
{
    public class WalletClient
    {
        private const string PrivateKey = "otjc193!tNT";
        private const string BaseUrl = "https://localhost:7174/Funds";

        public async Task CreatePlayer(string partnerId, string externalId, string timeStamp)
        {
            var sig = SignatureHelper.GenerateSignature(timeStamp, externalId, PrivateKey);

            var requestBody = new
            {
                partnerId,
                externalId,
                timeStamp,
                sig
            };

            await PostRequest("/CreatePlayer", requestBody);
        }

        public async Task GetBalance(string partnerId, string externalId, string timeStamp)
        {
            var sig = SignatureHelper.GenerateSignature(timeStamp, externalId, PrivateKey);

            var requestBody = new
            {
                partnerId,
                externalId,
                timeStamp,
                sig
            };

            await PostRequest("/GetBalance", requestBody);
        }

        public async Task Transfer(
            string partnerId,
            string externalId,
            string timeStamp,
            decimal amount,
            string transactionType,
            long extTransactionId,
            string currencyCode,
            string transactionDescription)
        {
            var sig = SignatureHelper.GenerateSignature(timeStamp, externalId, PrivateKey);

            var requestBody = new
            {
                partnerId,
                externalId,
                timeStamp,
                sig,
                amount,
                transactionType,
                extTransactionId,
                currencyCode,
                transactionDescription
            };

            await PostRequest("/Transfer", requestBody);
        }

        private async Task PostRequest(string endpoint, object requestBody)
        {
            string url = BaseUrl + endpoint;

            using var client = new HttpClient();

            string json = JsonSerializer.Serialize(requestBody);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync(url, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"POST {endpoint}");
                Console.WriteLine($"Status Code: {response.StatusCode}");
                Console.WriteLine("Response:");
                Console.WriteLine(responseContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling {endpoint}: {ex.Message}");
            }
        }
    }
}
