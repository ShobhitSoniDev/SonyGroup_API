using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;                 // or System.Text.Json — swap if your project uses that instead
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Application.Transactions.Interfaces
{
    public interface IRazorpayServiceRepository
    {
        Task<RazorpayOrderResult> CreateOrderAsync(decimal amount, string currency, string receipt);
        bool VerifySignature(string razorpayOrderId, string razorpayPaymentId, string razorpaySignature);
    }

    public class RazorpayOrderResult
    {
        public bool Success { get; set; }
        public string? RazorpayOrderId { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
