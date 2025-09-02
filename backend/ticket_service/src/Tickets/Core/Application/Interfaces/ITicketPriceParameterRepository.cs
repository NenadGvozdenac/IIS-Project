using ticket_service.src.Tickets.Core.Domain.Entities;

namespace ticket_service.src.Tickets.Core.Application.Interfaces;

public interface ITicketPriceParameterRepository
{
    TicketPriceParameter? GetByMatchZoneAndUser(int matchId, int zoneId, int? userId = null);
    IEnumerable<TicketPriceParameter> GetByMatch(int matchId);
    IEnumerable<TicketPriceParameter> GetByZone(int zoneId);
    TicketPriceParameter? GetById(int id);
    TicketPriceParameter Create(TicketPriceParameter parameter);
    TicketPriceParameter Update(TicketPriceParameter parameter);
    bool Delete(int id);
}
