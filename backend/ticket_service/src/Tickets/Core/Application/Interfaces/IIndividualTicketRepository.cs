using ticket_service.src.Tickets.Core.Domain.Entities;

namespace ticket_service.src.Tickets.Core.Application.Interfaces;

public interface IIndividualTicketRepository
{
    IndividualTicket? GetByPurchaseOfferId(int purchaseOfferId);
    IEnumerable<IndividualTicket> GetByMatch(int matchId);
    IndividualTicket Create(IndividualTicket individualTicket);
    IndividualTicket Update(IndividualTicket individualTicket);
    void Delete(int purchaseOfferId);
    IEnumerable<IndividualTicket> GetAvailableByMatch(int matchId);
}
