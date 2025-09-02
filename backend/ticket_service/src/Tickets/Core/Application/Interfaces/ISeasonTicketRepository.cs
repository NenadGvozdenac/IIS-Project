using ticket_service.src.Tickets.Core.Domain.Entities;

namespace ticket_service.src.Tickets.Core.Application.Interfaces;

public interface ISeasonTicketRepository
{
    SeasonTicket? GetByPurchaseOfferId(int purchaseOfferId);
    IEnumerable<SeasonTicket> GetBySeason(int seasonId);
    SeasonTicket Create(SeasonTicket seasonTicket);
    SeasonTicket Update(SeasonTicket seasonTicket);
    void Delete(int purchaseOfferId);
}
