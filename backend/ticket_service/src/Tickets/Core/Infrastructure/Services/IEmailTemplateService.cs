using ticket_service.src.Tickets.Core.Domain.Entities.Relational;

namespace ticket_service.src.Tickets.Core.Infrastructure.Services
{
    public interface IEmailTemplateService
    {
        Task<string> GenerateTicketPurchaseConfirmationAsync(User user, Cart cart, decimal totalAmount);
    }
}