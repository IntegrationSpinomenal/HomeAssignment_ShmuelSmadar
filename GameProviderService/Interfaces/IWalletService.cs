using System.Collections.Concurrent;
using GameProviderService.Models;

public interface IWalletService
{
    bool CreatePlayer(string externalId);
    bool TryGetBalance(string externalId, out long balance);
    TransferResponse ProcessTransaction(TransferRequest request);
}