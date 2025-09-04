namespace GameProviderService.Models
{
    public class BaseRequest
    {
        public string PartnerId { get; set; }
        public string ExternalId { get; set; }
        public string TimeStamp { get; set; }
        public string Sig { get; set; }
    }

    public class CreatePlayerRequest : BaseRequest { }

    public class GetBalanceRequest : BaseRequest { }

    public class TransferRequest : BaseRequest
    {
        public long Amount { get; set; }
        public string TransactionType { get; set; }
        public long ExtTransactionId { get; set; }
        public string CurrencyCode { get; set; }
        public string TransactionDescription { get; set; }
    }

    public class CreatePlayerResponse
    {
        public bool IsNew { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class GetBalanceResponse
    {
        public long Balance { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class TransferResponse
    {
        public long? TransactionId { get; set; }
        public long Balance { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsNew { get; set; }
    }
}
