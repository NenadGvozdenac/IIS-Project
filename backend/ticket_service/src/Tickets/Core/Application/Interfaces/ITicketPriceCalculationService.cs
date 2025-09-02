using ticket_service.src.Tickets.Core.Domain.Entities;

namespace ticket_service.src.Tickets.Core.Application.Interfaces;

public interface ITicketPriceCalculationService
{
    decimal CalculateTicketPrice(int matchId, int zoneId);
}
