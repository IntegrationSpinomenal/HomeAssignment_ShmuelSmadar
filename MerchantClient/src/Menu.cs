using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MerchantClient.src
{
    public class Menu
    {
        private readonly WalletActions _walletActions = new WalletActions();

        private readonly string partnerId;
        private readonly string externalId;
        private readonly string currencyCode;

        public Menu()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            partnerId = configuration["PartnerInfo:PartnerId"];
            externalId = configuration["PartnerInfo:ExternalId"];
            currencyCode = configuration["PartnerInfo:CurrencyCode"];
        }

        public async Task RunAsync()
        {
            while (true)
            {
                Console.WriteLine("\n=== Wallet Client Menu ===");
                Console.WriteLine("1. Create Player");
                Console.WriteLine("2. Add Funds (Deposit)");
                Console.WriteLine("3. Withdraw Funds");
                Console.WriteLine("4. Get Balance");
                Console.WriteLine("5. Exit");
                Console.Write("Select an option (1-5): ");

                var choice = Console.ReadLine()?.Trim();

                if (choice == "5")
                {
                    Console.WriteLine("Exiting...");
                    break;
                }

                string timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Creating Player...");
                        await _walletActions.CreatePlayerAsync(partnerId, externalId, timeStamp);
                        break;

                    case "2":
                        Console.Write("Enter amount to deposit: ");
                        if (decimal.TryParse(Console.ReadLine(), out var depositAmount) && depositAmount > 0)
                        {
                            long extTransactionId = DateTime.UtcNow.Ticks;
                            string transactionDescription = "Deposit via client";
                            string transactionType = "DEPOSIT";

                            await _walletActions.TransferAsync(
                                partnerId,
                                externalId,
                                timeStamp,
                                depositAmount,
                                transactionType,
                                extTransactionId,
                                currencyCode,
                                transactionDescription
                            );
                        }
                        else
                        {
                            Console.WriteLine("Invalid amount.");
                        }
                        break;

                    case "3":
                        Console.Write("Enter amount to withdraw: ");
                        if (decimal.TryParse(Console.ReadLine(), out var withdrawAmount) && withdrawAmount > 0)
                        {
                            long extTransactionId = DateTime.UtcNow.Ticks;
                            string transactionDescription = "Withdraw via client";
                            string transactionType = "WITHDRAW";

                            await _walletActions.TransferAsync(
                                partnerId,
                                externalId,
                                timeStamp,
                                withdrawAmount,
                                transactionType,
                                extTransactionId,
                                currencyCode,
                                transactionDescription
                            );
                        }
                        else
                        {
                            Console.WriteLine("Invalid amount.");
                        }
                        break;

                    case "4":
                        Console.WriteLine("Getting balance...");
                        await _walletActions.GetBalanceAsync(partnerId, externalId, timeStamp);
                        break;

                    default:
                        Console.WriteLine("Invalid choice, please select 1-5.");
                        break;
                }
            }
        }
    }
}
