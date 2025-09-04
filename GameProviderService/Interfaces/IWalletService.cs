using System.Collections.Concurrent;
using GameProviderService.Models;

public interface IWalletService
{
    bool CreatePlayer(string externalId);
    bool TryGetBalance(string externalId, out long balance);
    TransferResponse ProcessTransaction(TransferRequest request);
}

public class InMemoryWalletService : IWalletService
{
    private class Player
    {
        public long Balance { get; set; } = 0;
    }

    private ConcurrentDictionary<string, Player> players = new();

    public bool CreatePlayer(string externalId)
    {
        // Try to add new player; returns true if added, false if existed
        return players.TryAdd(externalId, new Player());
    }

    public bool TryGetBalance(string externalId, out long balance)
    {
        if (players.TryGetValue(externalId, out var player))
        {
            balance = player.Balance;
            return true;
        }
        balance = 0;
        return false;
    }

    public TransferResponse ProcessTransaction(TransferRequest request)
    {
        if (!players.TryGetValue(request.ExternalId, out var player))
        {
            return new TransferResponse
            {
                ErrorCode = 6004,
                ErrorMessage = "Player not found",
                Balance = 0,
                TransactionId = null,
                IsNew = false
            };
        }

        if (request.TransactionType == "DEPOSIT")
        {
            player.Balance += request.Amount;
        }
        else if (request.TransactionType == "WITHDRAW")
        {
            if (player.Balance < request.Amount)
            {
                return new TransferResponse
                {
                    ErrorCode = 6005,
                    ErrorMessage = "Insufficient balance",
                    Balance = player.Balance,
                    TransactionId = null,
                    IsNew = false
                };
            }
            player.Balance -= request.Amount;
        }
        else
        {
            return new TransferResponse
            {
                ErrorCode = 6006,
                ErrorMessage = "Invalid transaction type",
                Balance = player.Balance,
                TransactionId = null,
                IsNew = false
            };
        }

        return new TransferResponse
        {
            ErrorCode = 0,
            ErrorMessage = null,
            Balance = player.Balance,
            TransactionId = request.ExtTransactionId,
            IsNew = false
        };
    }
}
