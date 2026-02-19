using MaillotStore.Models;
using MaillotStore.Data; // Added to access the Database
using Microsoft.EntityFrameworkCore; // Added for database queries
using System.Text;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace MaillotStore.Services.Implementations
{
    public class WhatsAppService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WhatsAppService> _logger;
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory; // Added to query the DB safely in the background

        public WhatsAppService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<WhatsAppService> logger,
            IDbContextFactory<ApplicationDbContext> dbFactory) // Injected here
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _dbFactory = dbFactory;
        }

        public async Task SendOrderNotificationsAsync(Order order)
        {
            try
            {
                var apiKey = _configuration["WaSender:ApiKey"];
                var instanceId = _configuration["WaSender:InstanceId"];

                // ---------------------------------------------------------
                // 1. SEND CUSTOMER RECEIPT FIRST (Instant)
                // ---------------------------------------------------------
                if (!string.IsNullOrEmpty(order.CustomerPhone))
                {
                    var sbUser = new StringBuilder();
                    sbUser.AppendLine($"👋 Hello {order.CustomerName}!");
                    sbUser.AppendLine();
                    sbUser.AppendLine($"✅ We have received your order *#{order.Id}*.");
                    sbUser.AppendLine($"💰 Total Paid: {order.TotalAmount:N0} FCFA");
                    sbUser.AppendLine();
                    sbUser.AppendLine("Your items:");

                    if (order.OrderItems != null)
                    {
                        foreach (var item in order.OrderItems)
                        {
                            sbUser.AppendLine($"- {item.Product?.Name} (Size: {item.Size})");
                        }
                    }

                    sbUser.AppendLine();
                    sbUser.AppendLine($"We are preparing your package now.");
                    sbUser.AppendLine($"Thank you for choosing Maillot Store! ⚽");

                    string customerMessage = sbUser.ToString();
                    await SendMessageAsync(order.CustomerPhone, customerMessage, apiKey, instanceId);
                }

                // ---------------------------------------------------------
                // RATE LIMIT HANDLING (Free Trial = 1 msg / min)
                // ---------------------------------------------------------
                await Task.Delay(62000);

                // ---------------------------------------------------------
                // 2. DYNAMICALLY FETCH ADMIN PHONE NUMBER FROM DATABASE
                // ---------------------------------------------------------
                string adminPhone = string.Empty;
                using (var context = _dbFactory.CreateDbContext())
                {
                    // Option A: Try to get it from the AdminSettings table (Store Settings)
                    var adminSetting = await context.AdminSettings.FirstOrDefaultAsync(s => s.Key == "AdminWhatsAppNumber");
                    if (adminSetting != null && !string.IsNullOrWhiteSpace(adminSetting.Value))
                    {
                        adminPhone = adminSetting.Value;
                    }

                    // Option B: If not in settings, find the Account Phone Number of the user with the "Admin" role
                    if (string.IsNullOrWhiteSpace(adminPhone))
                    {
                        var adminUser = await context.Users
                            .Where(u => context.UserRoles.Any(ur => ur.UserId == u.Id &&
                                        context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Admin")))
                            .FirstOrDefaultAsync();

                        if (adminUser != null && !string.IsNullOrWhiteSpace(adminUser.PhoneNumber))
                        {
                            adminPhone = adminUser.PhoneNumber;
                        }
                    }

                    // Option C: Absolute Fallback to appsettings.json
                    if (string.IsNullOrWhiteSpace(adminPhone))
                    {
                        adminPhone = _configuration["WaSender:AdminPhoneNumber"];
                    }
                }

                if (string.IsNullOrWhiteSpace(adminPhone))
                {
                    _logger.LogWarning("Could not find an Admin phone number in the Database or config. Admin notification skipped.");
                    return;
                }

                // ---------------------------------------------------------
                // 3. SEND ADMIN MESSAGE
                // ---------------------------------------------------------
                var sbAdmin = new StringBuilder();
                sbAdmin.AppendLine($"🔔 *New Order #{order.Id}* 🔔");
                sbAdmin.AppendLine($"👤 Customer Name: {order.CustomerName}");
                sbAdmin.AppendLine($"📱 Phone Number: {order.CustomerPhone}");
                sbAdmin.AppendLine($"💰 Total Amount: {order.TotalAmount:N0} FCFA");
                sbAdmin.AppendLine($"💳 Payment Method: {order.PaymentMethod}");
                sbAdmin.AppendLine($"✅ Status: {order.PaymentStatus}");
                if (!string.IsNullOrEmpty(order.CustomerAddress)) sbAdmin.AppendLine($"📍 {order.CustomerAddress}");
                sbAdmin.AppendLine();
                sbAdmin.AppendLine("*📦 ORDER DETAILS:*");

                if (order.OrderItems != null)
                {
                    foreach (var item in order.OrderItems)
                    {
                        sbAdmin.AppendLine($"----------------");
                        sbAdmin.AppendLine($"👕 *{item.Product?.Name ?? "Unknown Product"}*");
                        sbAdmin.AppendLine($"   • Size: {item.Size}");
                        sbAdmin.AppendLine($"   • Qty: {item.Quantity}");

                        if (item.Product != null)
                        {
                            if (!string.IsNullOrEmpty(item.Product.Version))
                                sbAdmin.AppendLine($"   • Type: {item.Product.Version}");

                            if (!string.IsNullOrEmpty(item.Product.Season))
                                sbAdmin.AppendLine($"   • Season: {item.Product.Season}");
                        }

                        if (!string.IsNullOrEmpty(item.CustomName) || item.CustomNumber.HasValue)
                        {
                            sbAdmin.AppendLine($"   • ✏️ *Custom:* {item.CustomName} #{item.CustomNumber}");
                        }
                    }
                }

                string adminMessage = sbAdmin.ToString();
                await SendMessageAsync(adminPhone, adminMessage, apiKey, instanceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send WhatsApp notifications.");
            }
        }

        private async Task SendMessageAsync(string phoneNumber, string message, string apiKey, string instanceId)
        {
            try
            {
                string formattedPhone = FormatPhoneNumber(phoneNumber);

                var payload = new
                {
                    session = instanceId,
                    to = formattedPhone,
                    text = message
                };

                if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                {
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                }

                _logger.LogInformation($"Attempting to send WhatsApp to {formattedPhone}...");

                var response = await _httpClient.PostAsJsonAsync("https://wasenderapi.com/api/send-message", payload);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"❌ WaSender API Failed: {response.StatusCode} | Response: {responseBody}");
                }
                else
                {
                    _logger.LogInformation($"✅ WhatsApp sent successfully to {formattedPhone}!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending message to {phoneNumber}");
            }
        }

        private string FormatPhoneNumber(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return "";

            string clean = Regex.Replace(phone, @"[^\d]", "");

            if (clean.Length == 9) return "237" + clean;

            return clean;
        }
    }
}