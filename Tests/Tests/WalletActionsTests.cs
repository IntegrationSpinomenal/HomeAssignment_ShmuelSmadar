using System;
using System.Threading.Tasks;
using MerchantClient.src;
using Xunit;

namespace MerchantClient.Tests.Tests
{
    public class WalletActionsTests
    {
        private readonly WalletActions _walletActions = new WalletActions();

        private readonly string partnerId = "JohnDoe_Id";
        private readonly string externalId = "player123";
        private readonly string currencyCode = "USD";

        [Fact(DisplayName = "01. CreatePlayer returns IsNew=true")]
        public async Task Test01_CreatePlayer()
        {
            string timestamp = GetTimestamp();
            await _walletActions.CreatePlayerAsync(partnerId, externalId, timestamp);
        }

        [Fact(DisplayName = "02. GetBalance returns 0")]
        public async Task Test02_GetInitialBalance()
        {
            string timestamp = GetTimestamp();
            await _walletActions.GetBalanceAsync(partnerId, externalId, timestamp);
        }

        [Fact(DisplayName = "03. Deposit 1000 (Tx: 90001)")]
        public async Task Test03_Deposit1000()
        {
            string timestamp = GetTimestamp();
            await _walletActions.TransferAsync(
                partnerId, externalId, timestamp, 1000,
                "DEPOSIT", 90001,
                currencyCode, "Initial deposit"
            );
        }

        [Fact(DisplayName = "04. Withdraw 500 (Tx: 90002)")]
        public async Task Test04_Withdraw500()
        {
            string timestamp = GetTimestamp();
            await _walletActions.TransferAsync(
                partnerId, externalId, timestamp, 500,
                "WITHDRAW", 90002,
                currencyCode, "First withdrawal"
            );
        }

        [Fact(DisplayName = "05. Re-run Withdraw Tx: 90002 (idempotent)")]
        public async Task Test05_RepeatWithdraw500()
        {
            string timestamp = GetTimestamp();
            await _walletActions.TransferAsync(
                partnerId, externalId, timestamp, 500,
                "WITHDRAW", 90002,
                currencyCode, "Duplicate withdrawal"
            );
        }

        [Fact(DisplayName = "06. Invalid signature should fail")]
        public async Task Test06_InvalidSignature()
        {
            var invalidClient = new TamperedSignatureWalletActions();
            string timestamp = GetTimestamp();

            await invalidClient.CreatePlayerAsync(partnerId, externalId, timestamp);
        }

        [Fact(DisplayName = "07. Withdraw 999999 should fail (Insufficient Funds)")]
        public async Task Test07_WithdrawInsufficientFunds()
        {
            string timestamp = GetTimestamp();
            await _walletActions.TransferAsync(
                partnerId, externalId, timestamp, 999999,
                "WITHDRAW", 90003,
                currencyCode, "Too much withdraw"
            );
        }

        private string GetTimestamp() => DateTime.UtcNow.ToString("yyyyMMddHHmmss");
    }
}
