using Microsoft.EntityFrameworkCore;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;

namespace scouting_service.src.Scoutings.Core.Infrastructure.Repositories;

public class SessionStatusRepository : ISessionStatusRepository
{
    private readonly ScoutingDbContext _scoutingDbContext;

    public SessionStatusRepository(ScoutingDbContext context)
    {
        _scoutingDbContext = context;
    }

    public SessionStatus? GetById(int id)
    {
        return _scoutingDbContext.SessionStatuses
            .Include(ss => ss.Sessions)
            .FirstOrDefault(ss => ss.IdSessionStatus == id);
    }

    public IEnumerable<SessionStatus> GetAll()
    {
        return _scoutingDbContext.SessionStatuses
            .Include(ss => ss.Sessions)
            .ToList();
    }

    public SessionStatus? GetByName(string name)
    {
        return _scoutingDbContext.SessionStatuses
            .Include(ss => ss.Sessions)
            .FirstOrDefault(ss => ss.Name == name);
    }

    public SessionStatus Create(SessionStatus sessionStatus)
    {
        _scoutingDbContext.SessionStatuses.Add(sessionStatus);
        _scoutingDbContext.SaveChanges();
        return sessionStatus;
    }

    public SessionStatus Update(SessionStatus sessionStatus)
    {
        _scoutingDbContext.SessionStatuses.Update(sessionStatus);
        _scoutingDbContext.SaveChanges();
        return sessionStatus;
    }

    public bool Delete(int id)
    {
        var sessionStatus = _scoutingDbContext.SessionStatuses.Find(id);
        if (sessionStatus == null) return false;

        _scoutingDbContext.SessionStatuses.Remove(sessionStatus);
        _scoutingDbContext.SaveChanges();
        return true;
    }
}
