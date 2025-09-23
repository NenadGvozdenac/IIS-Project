using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;

namespace scouting_service.src.Scoutings.Core.Infrastructure.Repositories;

public class SessionTypeRepository : ISessionTypeRepository
{
    private readonly ScoutingDbContext _context;

    public SessionTypeRepository(ScoutingDbContext context)
    {
        _context = context;
    }

    public IEnumerable<SessionType> GetAll()
    {
        return _context.SessionTypes.ToList();
    }

    public SessionType? GetById(int id)
    {
        return _context.SessionTypes.Find(id);
    }

    public SessionType Create(SessionType sessionType)
    {
        _context.SessionTypes.Add(sessionType);
        _context.SaveChanges();
        return sessionType;
    }

    public SessionType Update(SessionType sessionType)
    {
        _context.SessionTypes.Update(sessionType);
        _context.SaveChanges();
        return sessionType;
    }

    public void Delete(int id)
    {
        var sessionType = _context.SessionTypes.Find(id);
        if (sessionType != null)
        {
            _context.SessionTypes.Remove(sessionType);
            _context.SaveChanges();
        }
    }
}
