using ArchitecturalWebsite.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ArchitecturalWebsite.Application.Services
{
    public class PaystackService : IPaystackService
    {
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;
 
        public PaystackService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _secretKey = configuration["Paystack:SecretKey"]!;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_secretKey}");
        }

        public async Task<string> CreatePaymentLink(string email, decimal amount, string reference, object metadata)
        {
            var requestBody = new
            {
                email = email,
                amount = (int)(amount * 100), // Paystack expects kobo
                reference = reference,
                metadata = metadata,
                channels = new[] { "card", "bank_transfer" }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("transaction/initialize", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Paystack API error: {responseString}");
            }

            var result = JsonSerializer.Deserialize<PaystackInitializeResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result?.Status != true || result.Data == null)
            {
                throw new Exception($"Paystack initialization failed: {result?.Message}");
            }

            return result.Data.AuthorizationUrl;
        }
    }

    public class PaystackInitializeResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public PaystackInitializeData Data { get; set; } = new();
    }

    public class PaystackInitializeData
    {
        [JsonPropertyName("authorization_url")]
        public string AuthorizationUrl { get; set; } = string.Empty;

        [JsonPropertyName("access_code")]
        public string AccessCode { get; set; } = string.Empty;

        public string Reference { get; set; } = string.Empty;
    }
}