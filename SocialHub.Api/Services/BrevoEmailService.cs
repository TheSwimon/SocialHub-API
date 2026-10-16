using Microsoft.Extensions.Options;
using SocialHub.Api.Settings;

namespace SocialHub.Api.Services
{
    public class BrevoEmailService : IEmailService
    {
        private readonly HttpClient httpClient;
        private readonly EmailSettings emailSettings;

        public BrevoEmailService(HttpClient httpClient, IOptions<EmailSettings> emailOptions)
        {
            this.httpClient = httpClient;
            emailSettings = emailOptions.Value;
        }

        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            var payload = new
            {
                sender = new { name = emailSettings.FromName, email = emailSettings.FromAddress },
                to = new[] { new { email = to } },
                subject,
                htmlContent = htmlBody
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post, "https://api.brevo.com/v3/smtp/email")
            {
                Content = JsonContent.Create(payload)
            };

            request.Headers.Add("api-key", emailSettings.ApiKey);

            using var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = response.Content.ReadAsStringAsync();
                throw new Exception($"{(int)response.StatusCode} {error}");
            }

        }
    }
}
