using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

namespace ticket_service.src.Tickets.Core.Infrastructure.Services
{
    public class GmailEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public GmailEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpSection = _configuration.GetSection("Smtp");
            var smtpUser = smtpSection["User"];
            var smtpPass = smtpSection["Password"];
            var smtpHost = smtpSection["Host"] ?? "smtp.gmail.com";
            var smtpPort = int.TryParse(smtpSection["Port"], out var port) ? port : 587;

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mail = new MailMessage(smtpUser, to, subject, body);
            mail.IsBodyHtml = true;
            await client.SendMailAsync(mail);
        }
    }
}
