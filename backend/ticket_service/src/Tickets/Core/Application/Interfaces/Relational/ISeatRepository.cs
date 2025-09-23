using ticket_service.src.Tickets.Core.Domain.Entities.Relational;

namespace ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

public interface ISeatRepository
{
    Seat? GetById(int id);
    IEnumerable<Seat> GetAll();
    IEnumerable<Seat> GetByZoneId(int zoneId);
    Seat? GetByZoneAndPosition(int zoneId, int row, int number);
    Seat? GetByZoneAndPositionAndDirection(int zoneId, int row, int number, string direction);
    Seat Create(Seat seat);
    Seat Update(Seat seat);
    bool Delete(int id);
    IEnumerable<Seat> GetEnabledSeats();
}
