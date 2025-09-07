using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;

namespace scouting_service.src.Scoutings.Core.Infrastructure.Repositories;

public class SessionMetricRepository : ISessionMetricRepository
{
    private readonly ScoutingDbContext _context;

    public SessionMetricRepository(ScoutingDbContext context)
    {
        _context = context;
    }

    public IEnumerable<SessionMetric> GetAll()
    {
        return _context.SessionMetrics.ToList();
    }

    public SessionMetric? GetById(int id)
    {
        return _context.SessionMetrics.Find(id);
    }

    public SessionMetric? GetBySessionAndMetric(int sessionId, int metricId)
    {
        return _context.SessionMetrics
            .FirstOrDefault(sm => sm.IdSession == sessionId && sm.IdMetrics == metricId);
    }

    public SessionMetric Create(SessionMetric sessionMetric)
    {
        _context.SessionMetrics.Add(sessionMetric);
        _context.SaveChanges();
        return sessionMetric;
    }

    public SessionMetric Update(SessionMetric sessionMetric)
    {
        _context.SessionMetrics.Update(sessionMetric);
        _context.SaveChanges();
        return sessionMetric;
    }

    public void Delete(int id)
    {
        var sessionMetric = _context.SessionMetrics.Find(id);
        if (sessionMetric != null)
        {
            _context.SessionMetrics.Remove(sessionMetric);
            _context.SaveChanges();
        }
    }

    public IEnumerable<SessionMetric> GetBySession(int sessionId)
    {
        return _context.SessionMetrics.Where(sm => sm.IdSession == sessionId).ToList();
    }

    public IEnumerable<SessionMetric> GetByMetric(int metricId)
    {
        return _context.SessionMetrics.Where(sm => sm.IdMetrics == metricId).ToList();
    }
}
