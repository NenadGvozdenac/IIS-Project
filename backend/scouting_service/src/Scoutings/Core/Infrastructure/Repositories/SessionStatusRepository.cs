using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;

namespace scouting_service.src.Scoutings.Core.Infrastructure.Repositories;

public class SessionStatusRepository : ISessionStatusRepository
{
    private readonly ScoutingDbContext _context;

    public SessionStatusRepository(ScoutingDbContext context)
    {
        _context = context;
    }

    public IEnumerable<SessionStatus> GetAll()
    {
        return _context.SessionStatuses.ToList();
    }

    public SessionStatus? GetById(int id)
    {
        return _context.SessionStatuses.Find(id);
    }

    public SessionStatus Create(SessionStatus sessionStatus)
    {
        _context.SessionStatuses.Add(sessionStatus);
        _context.SaveChanges();
        return sessionStatus;
    }

    public SessionStatus Update(SessionStatus sessionStatus)
    {
        _context.SessionStatuses.Update(sessionStatus);
        _context.SaveChanges();
        return sessionStatus;
    }

    public void Delete(int id)
    {
        var sessionStatus = _context.SessionStatuses.Find(id);
        if (sessionStatus != null)
        {
            _context.SessionStatuses.Remove(sessionStatus);
            _context.SaveChanges();
        }
    }
}
