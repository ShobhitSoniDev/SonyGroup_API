using Jewellery.Application.Transactions.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Infrastructure.Services
{
    public class RazorpayService : IRazorpayServiceRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _keyId;
        private readonly string _keySecret;

        private const string RazorpayBaseUrl = "https://api.razorpay.com/v1/";

        public RazorpayService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            //_keyId = configuration["Razorpay:KeyId"]
            //         ?? throw new InvalidOperationException("Razorpay:KeyId is not configured.");
            //_keySecret = configuration["Razorpay:KeySecret"]
            //         ?? throw new InvalidOperationException("Razorpay:KeySecret is not configured.");
            _keyId = "rzp_test_TT7Kir0Tr3J0bA";
            _keySecret = "1lNGapbGmJennorENSuyjevn";

            _httpClient.BaseAddress = new Uri(RazorpayBaseUrl);

            var authBytes = Encoding.UTF8.GetBytes($"{_keyId}:{_keySecret}");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
        }

        // ------------------------------------------------------------
        // Creates a Razorpay order. Amount must be passed in the
        // major currency unit (e.g. rupees) — converted to paise here.
        // ------------------------------------------------------------
        public async Task<RazorpayOrderResult> CreateOrderAsync(decimal amount, string currency, string receipt)
        {
            try
            {
                var payload = new
                {
                    amount = (int)Math.Round(amount * 100), // paise
                    currency = currency ?? "INR",
                    receipt = receipt,
                    payment_capture = 1
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("orders", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new RazorpayOrderResult
                    {
                        Success = false,
                        ErrorMessage = $"Razorpay order creation failed: {responseBody}"
                    };
                }

                dynamic result = JsonConvert.DeserializeObject(responseBody)!;
                string razorpayOrderId = result.id;

                return new RazorpayOrderResult
                {
                    Success = true,
                    RazorpayOrderId = razorpayOrderId
                };
            }
            catch (Exception ex)
            {
                return new RazorpayOrderResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        // ------------------------------------------------------------
        // Verifies Razorpay's checkout signature:
        // expected_signature = HMAC_SHA256(order_id + "|" + payment_id, key_secret)
        // Must equal the razorpay_signature returned by the checkout
        // widget's `handler` callback. NEVER trust the client for this.
        // ------------------------------------------------------------
        public bool VerifySignature(string razorpayOrderId, string razorpayPaymentId, string razorpaySignature)
        {
            if (string.IsNullOrWhiteSpace(razorpayOrderId) ||
                string.IsNullOrWhiteSpace(razorpayPaymentId) ||
                string.IsNullOrWhiteSpace(razorpaySignature))
            {
                return false;
            }

            var payload = $"{razorpayOrderId}|{razorpayPaymentId}";
            var keyBytes = Encoding.UTF8.GetBytes(_keySecret);
            var payloadBytes = Encoding.UTF8.GetBytes(payload);

            using var hmac = new HMACSHA256(keyBytes);
            var hash = hmac.ComputeHash(payloadBytes);
            var computedSignature = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

            return string.Equals(computedSignature, razorpaySignature, StringComparison.OrdinalIgnoreCase);
        }
    }
}
