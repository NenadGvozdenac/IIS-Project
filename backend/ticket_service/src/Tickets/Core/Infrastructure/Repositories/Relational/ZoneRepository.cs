using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;
using ticket_service.src.Tickets.Core.Domain.Entities.Relational;

namespace ticket_service.src.Tickets.Core.Infrastructure.Repositories.Relational;

public class ZoneRepository : IZoneRepository
{
    private readonly TicketDbContext _ticketDbContext;

    public ZoneRepository(TicketDbContext context)
    {
        _ticketDbContext = context;
    }

    public Zone? GetById(int id)
    {
        return _ticketDbContext.Zones.Find(id);
    }

    public IEnumerable<Zone> GetAll()
    {
        return _ticketDbContext.Zones.ToList();
    }

    public Zone Create(Zone zone)
    {
        _ticketDbContext.Zones.Add(zone);
        _ticketDbContext.SaveChanges();
        return zone;
    }

    public Zone Update(Zone zone)
    {
        _ticketDbContext.Zones.Update(zone);
        _ticketDbContext.SaveChanges();
        return zone;
    }

    public bool Delete(int id)
    {
        var zone = _ticketDbContext.Zones.Find(id);
        if (zone == null) return false;

        _ticketDbContext.Zones.Remove(zone);
        _ticketDbContext.SaveChanges();
        return true;
    }
}
