using System;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace SeemsAPIService.Application.Services
{
    public class EmailTriggerService
    {
        private readonly HttpClient _httpClient;

        public EmailTriggerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, string? ccEmail = null)
        {
            try
            {
                // 1️⃣ Build the request payload
                var emailRequest = new
                {
                    toEmail = new[] { toEmail },
                    subject = subject,
                    body = body,
                    CCEmail = string.IsNullOrEmpty(ccEmail) ? Array.Empty<string>() : new[] { ccEmail }
                };

                // 2️⃣ Serialize object to JSON
                var jsonPayload = JsonSerializer.Serialize(emailRequest);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // 3️⃣ Make POST request to external Email API
                var response = await _httpClient.PostAsync("https://localhost:7158/api/Email/send", content);

                // 4️⃣ Read response
                var responseText = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("✅ Email request sent successfully!");
                    Console.WriteLine($"Response: {responseText}");
                }
                else
                {
                    Console.WriteLine($"⚠️ Email service returned status: {response.StatusCode}");
                    Console.WriteLine($"Response: {responseText}");
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"❌ HTTP Request Error: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected Error: {ex.Message}");
            }
        }
    }
}