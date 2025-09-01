using ticket_service.src.Tickets.Core.Domain.Entities;

namespace ticket_service.src.Tickets.Core.Application.Interfaces;

public interface IZoneRepository
{
    Zone? GetById(int id);
    IEnumerable<Zone> GetAll();
    Zone Create(Zone zone);
    Zone Update(Zone zone);
    bool Delete(int id);
}
