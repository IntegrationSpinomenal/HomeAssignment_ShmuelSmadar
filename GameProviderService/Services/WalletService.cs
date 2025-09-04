using GameProviderService.Models;
using System.Collections.Concurrent;

namespace GameProviderService.Services;
public class WalletService : IWalletService
{
    private class PlayerData
    {
        public long Balance = 0;
        public HashSet<long> Transactions = new();
    }

    private readonly ConcurrentDictionary<string, PlayerData> _players = new();

    public bool CreatePlayer(string playerId)
    {
        return _players.TryAdd(playerId, new PlayerData());
    }

    public bool TryGetBalance(string playerId, out long balance)
    {
        if (_players.TryGetValue(playerId, out var player))
        {
            balance = player.Balance;
            return true;
        }

        balance = 0;
        return false;
    }

    public TransferResponse ProcessTransaction(TransferRequest request)
    {
        if (!_players.TryGetValue(request.ExternalId, out var player))
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

        if (request.Amount <= 0)
        {
            return new TransferResponse
            {
                ErrorCode = 6012,
                ErrorMessage = "Amount must be positive",
                Balance = player.Balance,
                TransactionId = null,
                IsNew = false
            };
        }
        lock (player)
        {
            if (player.Transactions.Contains(request.ExtTransactionId))
            {
                return new TransferResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = null,
                    Balance = player.Balance,
                    TransactionId = request.ExtTransactionId,
                    IsNew = false
                };
            }

            if (request.TransactionType.ToUpper() == "DEPOSIT")
            {
                player.Balance += request.Amount;
            }
            else if (request.TransactionType.ToUpper() == "WITHDRAW")
            {
                if (request.Amount > player.Balance)
                {
                    return new TransferResponse
                    {
                        ErrorCode = 6011,
                        ErrorMessage = "Insufficient Funds",
                        Balance = player.Balance,
                        TransactionId = null,
                        IsNew = false
                    };
                }
                player.Balance -= request.Amount;
            }

            player.Transactions.Add(request.ExtTransactionId);

            return new TransferResponse
            {
                ErrorCode = 0,
                ErrorMessage = null,
                Balance = player.Balance,
                TransactionId = request.ExtTransactionId,
                IsNew = true
            };
        }
    }
}