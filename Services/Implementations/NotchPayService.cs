using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

namespace MaillotStore.Services.Implementations
{
    public class NotchPayService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotchPayService> _logger;

        private readonly bool _isSimulationMode = false;

        public NotchPayService(HttpClient httpClient, IConfiguration configuration, ILogger<NotchPayService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            var baseUrl = _configuration["NotchPay:BaseUrl"] ?? "https://api.notchpay.co";
            _httpClient.BaseAddress = new Uri(baseUrl);

            var publicKey = _configuration["NotchPay:PublicKey"];
            if (!string.IsNullOrEmpty(publicKey))
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", publicKey);
            }
        }

        public async Task<NotchPayInitResponse?> InitializePaymentAsync(decimal amount, string email, string reference, string callbackUrl)
        {
            if (_isSimulationMode)
            {
                _logger.LogInformation($"[Simulation] Initializing payment for {reference}");
                return new NotchPayInitResponse
                {
                    AuthorizationUrl = $"{callbackUrl}?reference={reference}",
                    AccessCode = "SIMULATED_ACCESS_CODE",
                    Reference = reference
                };
            }

            try
            {
                var payload = new
                {
                    email = email,
                    currency = "XAF",
                    amount = (int)amount,
                    reference = reference,
                    callback = callbackUrl,
                    description = $"Payment for Order #{reference}"
                };

                var response = await _httpClient.PostAsJsonAsync("/payments/initialize", payload);
                var content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"NotchPay Init Response ({response.StatusCode}): {content}");

                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(content);
                    var root = doc.RootElement;

                    string? authUrl = null;

                    if (root.TryGetProperty("authorization_url", out var urlProp))
                        authUrl = urlProp.GetString();
                    else if (root.TryGetProperty("transaction", out var transProp) && transProp.TryGetProperty("authorization_url", out var transUrl))
                        authUrl = transUrl.GetString();
                    else if (root.TryGetProperty("payment", out var payProp) && payProp.TryGetProperty("authorization_url", out var payUrl))
                        authUrl = payUrl.GetString();

                    if (!string.IsNullOrEmpty(authUrl))
                    {
                        return new NotchPayInitResponse { AuthorizationUrl = authUrl, Reference = reference };
                    }
                }

                _logger.LogError($"Failed to parse authorization_url from NotchPay response. Status: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing NotchPay payment");
                return null;
            }
        }

        public async Task<string?> VerifyPaymentAsync(string reference)
        {
            if (_isSimulationMode) return "complete";

            try
            {
                var response = await _httpClient.GetAsync($"/payments/{reference}");

                // --- CRITICAL FIX: Handle 404 (Not Found) as a Cancellation ---
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning($"Payment reference {reference} not found (404). Treating as canceled.");
                    return "canceled";
                }
                // -------------------------------------------------------------

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"NotchPay Verify Response ({response.StatusCode}): {content}");

                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(content);
                    var root = doc.RootElement;

                    string? status = null;

                    if (root.TryGetProperty("transaction", out var trans) && trans.TryGetProperty("status", out var s1))
                        status = s1.GetString();
                    else if (root.TryGetProperty("payment", out var pay) && pay.TryGetProperty("status", out var s2))
                        status = s2.GetString();
                    else if (root.TryGetProperty("status", out var s3))
                        status = s3.GetString();

                    return status?.ToLower();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying NotchPay payment");
                return null;
            }
        }
    }

    public class NotchPayInitResponse
    {
        [JsonPropertyName("authorization_url")]
        public string AuthorizationUrl { get; set; }

        [JsonPropertyName("access_code")]
        public string AccessCode { get; set; }

        [JsonPropertyName("reference")]
        public string Reference { get; set; }
    }
}