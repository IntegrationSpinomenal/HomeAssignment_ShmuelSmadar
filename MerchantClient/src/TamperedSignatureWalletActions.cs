using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerchantClient.src
{
    public class TamperedSignatureWalletActions : WalletActions
    {
        public new async Task CreatePlayerAsync(string partnerId, string externalId, string timeStamp)
        {
            var sig = "invalidsignature";
            var requestBody = new
            {
                partnerId,
                externalId,
                timeStamp,
                sig
            };

            string endpoint = "https://localhost:7174/Funds/CreatePlayer";
            await new HttpHelper().PostAsync(endpoint, requestBody);
        }
    }
}
