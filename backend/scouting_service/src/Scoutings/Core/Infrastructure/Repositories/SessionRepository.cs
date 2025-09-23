using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace scouting_service.src.Scoutings.Core.Infrastructure.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly ScoutingDbContext _context;

    public SessionRepository(ScoutingDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Session> GetAll()
    {
        return _context.Sessions
            .Include(s => s.IdPlayerNavigation)
                .ThenInclude(p => p.IdNationalityNavigation)
            .Include(s => s.IdPlayerNavigation)
                .ThenInclude(p => p.IdPositionNavigation)
            .Include(s => s.IdSessionStatusNavigation)
            .Include(s => s.IdSessionTypeNavigation)
            .Include(s => s.IdUserNavigation)
            .Include(s => s.SessionMetrics)
            .ToList();
    }

    public Session? GetById(int id)
    {
        return _context.Sessions
            .Include(s => s.IdPlayerNavigation)
                .ThenInclude(p => p.IdNationalityNavigation)
            .Include(s => s.IdPlayerNavigation)
                .ThenInclude(p => p.IdPositionNavigation)
            .Include(s => s.IdSessionStatusNavigation)
            .Include(s => s.IdSessionTypeNavigation)
            .Include(s => s.IdUserNavigation)
            .Include(s => s.SessionMetrics)
            .FirstOrDefault(s => s.IdSession == id);
    }

    public Session Create(Session session)
    {
        _context.Sessions.Add(session);
        _context.SaveChanges();
        return session;
    }

    public Session Update(Session session)
    {
        _context.Sessions.Update(session);
        _context.SaveChanges();
        return session;
    }

    public void Delete(int id)
    {
        var session = _context.Sessions.Find(id);
        if (session != null)
        {
            _context.Sessions.Remove(session);
            _context.SaveChanges();
        }
    }

    public IEnumerable<Session> GetByPlayer(int playerId)
    {
        return _context.Sessions
            .Include(s => s.IdPlayerNavigation)
                .ThenInclude(p => p.IdNationalityNavigation)
            .Include(s => s.IdPlayerNavigation)
                .ThenInclude(p => p.IdPositionNavigation)
            .Include(s => s.IdSessionStatusNavigation)
            .Include(s => s.IdSessionTypeNavigation)
            .Include(s => s.IdUserNavigation)
            .Include(s => s.SessionMetrics)
            .Where(s => s.IdPlayer == playerId)
            .ToList();
    }

    public IEnumerable<Session> GetByUser(int userId)
    {
        return _context.Sessions
            .Include(s => s.IdPlayerNavigation)
                .ThenInclude(p => p.IdNationalityNavigation)
            .Include(s => s.IdPlayerNavigation)
                .ThenInclude(p => p.IdPositionNavigation)
            .Include(s => s.IdSessionStatusNavigation)
            .Include(s => s.IdSessionTypeNavigation)
            .Include(s => s.IdUserNavigation)
            .Include(s => s.SessionMetrics)
            .Where(s => s.IdUser == userId)
            .ToList();
    }

    public IEnumerable<Session> GetByStatus(int statusId)
    {
        return _context.Sessions
            .Include(s => s.IdPlayerNavigation)
                .ThenInclude(p => p.IdNationalityNavigation)
            .Include(s => s.IdPlayerNavigation)
                .ThenInclude(p => p.IdPositionNavigation)
            .Include(s => s.IdSessionStatusNavigation)
            .Include(s => s.IdSessionTypeNavigation)
            .Include(s => s.IdUserNavigation)
            .Include(s => s.SessionMetrics)
            .Where(s => s.IdSessionStatus == statusId)
            .ToList();
    }

    public IEnumerable<Session> GetByType(int typeId)
    {
        return _context.Sessions
            .Include(s => s.IdPlayerNavigation)
                .ThenInclude(p => p.IdNationalityNavigation)
            .Include(s => s.IdPlayerNavigation)
                .ThenInclude(p => p.IdPositionNavigation)
            .Include(s => s.IdSessionStatusNavigation)
            .Include(s => s.IdSessionTypeNavigation)
            .Include(s => s.IdUserNavigation)
            .Include(s => s.SessionMetrics)
            .Where(s => s.IdSessionType == typeId)
            .ToList();
    }
}
