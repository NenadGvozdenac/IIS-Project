using System.Threading.Tasks;

namespace ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}

