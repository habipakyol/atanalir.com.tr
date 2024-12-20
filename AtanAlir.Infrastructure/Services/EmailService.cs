using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtanAlir.Infrastructure.Services
{
    // AtanAlir.Infrastructure/Services/EmailService.cs
    public class EmailService
    {
        private readonly string _apiKey;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _apiKey = configuration["SendGrid:ApiKey"] ?? throw new ArgumentNullException("SendGrid:ApiKey");
            _logger = logger;
        }

        public async Task SendVerificationEmailAsync(string email, string code)
        {
            try
            {
                var client = new SendGridClient(_apiKey);
                var from = new EmailAddress("", ""); // Doğrulanmış email adresiniz
                var to = new EmailAddress(email);
                var subject = "AtanAlir - Email Doğrulama";
                var plainTextContent = $"Doğrulama kodunuz: {code}";
                var htmlContent = $"<strong>Doğrulama kodunuz:</strong> {code}";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);

                if (!response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Body.ReadAsStringAsync();
                    _logger.LogError("Email sending failed. Status Code: {StatusCode}, Body: {Body}",
                        response.StatusCode, responseBody);
                    throw new Exception($"Email sending failed: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email}", email);
                throw;
            }
        }
    }
}
