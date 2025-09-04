using GameProviderService.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;  // Add this import

namespace GameProviderService.Controllers
{
    [ApiController]
    [Route("Funds")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly string _privateKey;

        // Inject IConfiguration to access appsettings.json
        public WalletController(IWalletService walletService, IConfiguration configuration)
        {
            _walletService = walletService;
            _privateKey = configuration["Keys:PrivateKey"];
        }

        private bool IsValidSignature(string timeStamp, string externalId, string signature)
        {
            var input = timeStamp + externalId + _privateKey;
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            var expectedSig = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            return expectedSig == signature;
        }

        [HttpPost("CreatePlayer")]
        public IActionResult CreatePlayer([FromBody] CreatePlayerRequest request)
        {
            if (!IsValidSignature(request.TimeStamp, request.ExternalId, request.Sig))
            {
                return Ok(new CreatePlayerResponse
                {
                    ErrorCode = 6002,
                    ErrorMessage = "Invalid Signature",
                    IsNew = false
                });
            }

            var result = _walletService.CreatePlayer(request.ExternalId);

            return Ok(new CreatePlayerResponse
            {
                ErrorCode = 0,
                ErrorMessage = null,
                IsNew = result
            });
        }

        [HttpPost("GetBalance")]
        public IActionResult GetBalance([FromBody] GetBalanceRequest request)
        {
            if (!IsValidSignature(request.TimeStamp, request.ExternalId, request.Sig))
            {
                return Ok(new GetBalanceResponse
                {
                    ErrorCode = 6002,
                    ErrorMessage = "Invalid Signature",
                    Balance = 0
                });
            }

            var success = _walletService.TryGetBalance(request.ExternalId, out var balance);

            if (!success)
            {
                return Ok(new GetBalanceResponse
                {
                    ErrorCode = 6004,
                    ErrorMessage = "Player not found",
                    Balance = 0
                });
            }

            return Ok(new GetBalanceResponse
            {
                ErrorCode = 0,
                ErrorMessage = null,
                Balance = balance
            });
        }

        [HttpPost("Transfer")]
        public IActionResult Transfer([FromBody] TransferRequest request)
        {
            if (!IsValidSignature(request.TimeStamp, request.ExternalId, request.Sig))
            {
                return Ok(new TransferResponse
                {
                    ErrorCode = 6002,
                    ErrorMessage = "Invalid Signature",
                    Balance = 0,
                    TransactionId = null,
                    IsNew = false
                });
            }

            var result = _walletService.ProcessTransaction(request);

            return Ok(result);
        }
    }
}
