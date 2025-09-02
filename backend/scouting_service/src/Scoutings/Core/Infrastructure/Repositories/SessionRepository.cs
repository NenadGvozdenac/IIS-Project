using Microsoft.EntityFrameworkCore;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;

namespace scouting_service.src.Scoutings.Core.Infrastructure.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly ScoutingDbContext _scoutingDbContext;

    public SessionRepository(ScoutingDbContext context)
    {
        _scoutingDbContext = context;
    }

    public Session? GetById(int id)
    {
        return _scoutingDbContext.Sessions
            .Include(s => s.IdPlayerNavigation)
            .Include(s => s.IdSessionStatusNavigation)
            .Include(s => s.IdSessionTypeNavigation)
            .Include(s => s.SessionMetrics)
            .FirstOrDefault(s => s.IdSession == id);
    }

    public IEnumerable<Session> GetAll()
    {
        return _scoutingDbContext.Sessions
            .Include(s => s.IdPlayerNavigation)
            .Include(s => s.IdSessionStatusNavigation)
            .Include(s => s.IdSessionTypeNavigation)
            .Include(s => s.SessionMetrics)
            .ToList();
    }

    public IEnumerable<Session> GetByPlayerId(int playerId)
    {
        return _scoutingDbContext.Sessions
            .Include(s => s.IdPlayerNavigation)
            .Include(s => s.IdSessionStatusNavigation)
            .Include(s => s.IdSessionTypeNavigation)
            .Include(s => s.SessionMetrics)
            .Where(s => s.IdPlayer == playerId)
            .ToList();
    }

    public IEnumerable<Session> GetByUserId(int userId)
    {
        return _scoutingDbContext.Sessions
            .Include(s => s.IdPlayerNavigation)
            .Include(s => s.IdSessionStatusNavigation)
            .Include(s => s.IdSessionTypeNavigation)
            .Include(s => s.SessionMetrics)
            .Where(s => s.IdUser == userId)
            .ToList();
    }

    public IEnumerable<Session> GetBySessionType(int sessionTypeId)
    {
        return _scoutingDbContext.Sessions
            .Include(s => s.IdPlayerNavigation)
            .Include(s => s.IdSessionStatusNavigation)
            .Include(s => s.IdSessionTypeNavigation)
            .Include(s => s.SessionMetrics)
            .Where(s => s.IdSessionType == sessionTypeId)
            .ToList();
    }

    public IEnumerable<Session> GetBySessionStatus(int sessionStatusId)
    {
        return _scoutingDbContext.Sessions
            .Include(s => s.IdPlayerNavigation)
            .Include(s => s.IdSessionStatusNavigation)
            .Include(s => s.IdSessionTypeNavigation)
            .Include(s => s.SessionMetrics)
            .Where(s => s.IdSessionStatus == sessionStatusId)
            .ToList();
    }

    public Session Create(Session session)
    {
        _scoutingDbContext.Sessions.Add(session);
        _scoutingDbContext.SaveChanges();
        return session;
    }

    public Session Update(Session session)
    {
        _scoutingDbContext.Sessions.Update(session);
        _scoutingDbContext.SaveChanges();
        return session;
    }

    public bool Delete(int id)
    {
        var session = _scoutingDbContext.Sessions.Find(id);
        if (session == null) return false;

        _scoutingDbContext.Sessions.Remove(session);
        _scoutingDbContext.SaveChanges();
        return true;
    }
}
