using System;
using System.Threading.Tasks;

namespace MerchantClient.src
{
    public class WalletActions
    {
        private const string PrivateKey = "otjc193!tNT";
        private const string BaseUrl = "https://localhost:7174/Funds";

        private readonly HttpHelper _httpHelper = new HttpHelper(maxRetries: 3, delayMilliseconds: 1000);

        public async Task CreatePlayerAsync(string partnerId, string externalId, string timeStamp)
        {
            var sig = SignatureHelper.GenerateSignature(timeStamp, externalId, PrivateKey);
            var requestBody = new
            {
                partnerId,
                externalId,
                timeStamp,
                sig
            };

            string endpoint = BaseUrl + "/CreatePlayer";
            await _httpHelper.PostAsync(endpoint, requestBody);
        }

        public async Task GetBalanceAsync(string partnerId, string externalId, string timeStamp)
        {
            var sig = SignatureHelper.GenerateSignature(timeStamp, externalId, PrivateKey);
            var requestBody = new
            {
                partnerId,
                externalId,
                timeStamp,
                sig
            };

            string endpoint = BaseUrl + "/GetBalance";
            await _httpHelper.PostAsync(endpoint, requestBody);
        }

        public async Task TransferAsync(
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

            string endpoint = BaseUrl + "/Transfer";
            await _httpHelper.PostAsync(endpoint, requestBody);
        }
    }
}
