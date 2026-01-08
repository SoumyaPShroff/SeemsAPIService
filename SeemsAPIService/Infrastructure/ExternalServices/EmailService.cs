using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SeemsAPIService.Application.Interfaces;

namespace SeemsAPIService.Infrastructure.ExternalServices
{
    public class EmailService : IEmailService
    {
        private readonly HttpClient _httpClient;

        public EmailService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string body,
            string? ccEmail = null)
        {
            var emailRequest = new
            {
                toEmail = new[] { toEmail },
                subject = subject,
                body = body,
                CCEmail = string.IsNullOrEmpty(ccEmail)
                            ? Array.Empty<string>()
                            : new[] { ccEmail }
            };

            var jsonPayload = JsonSerializer.Serialize(emailRequest);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(
                    "https://localhost:7158/api/Email/send",
                    content
                );

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new ApplicationException(
                        $"Email API failed: {response.StatusCode} - {error}");
                }
            }
            catch (Exception)
            {
                // let upper layers decide how to handle
                throw;
            }
        }
    }
}
