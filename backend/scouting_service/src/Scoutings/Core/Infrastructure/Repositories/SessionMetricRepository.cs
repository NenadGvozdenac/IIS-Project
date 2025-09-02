using Microsoft.EntityFrameworkCore;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;

namespace scouting_service.src.Scoutings.Core.Infrastructure.Repositories;

public class SessionMetricRepository : ISessionMetricRepository
{
    private readonly ScoutingDbContext _scoutingDbContext;

    public SessionMetricRepository(ScoutingDbContext context)
    {
        _scoutingDbContext = context;
    }

    public SessionMetric? GetById(int id)
    {
        return _scoutingDbContext.SessionMetrics
            .Include(sm => sm.IdSessionNavigation)
            .Include(sm => sm.IdMetricsNavigation)
            .FirstOrDefault(sm => sm.IdSessionMetrics == id);
    }

    public IEnumerable<SessionMetric> GetAll()
    {
        return _scoutingDbContext.SessionMetrics
            .Include(sm => sm.IdSessionNavigation)
            .Include(sm => sm.IdMetricsNavigation)
            .ToList();
    }

    public IEnumerable<SessionMetric> GetBySessionId(int sessionId)
    {
        return _scoutingDbContext.SessionMetrics
            .Include(sm => sm.IdSessionNavigation)
            .Include(sm => sm.IdMetricsNavigation)
            .Where(sm => sm.IdSession == sessionId)
            .ToList();
    }

    public IEnumerable<SessionMetric> GetByMetricId(int metricId)
    {
        return _scoutingDbContext.SessionMetrics
            .Include(sm => sm.IdSessionNavigation)
            .Include(sm => sm.IdMetricsNavigation)
            .Where(sm => sm.IdMetrics == metricId)
            .ToList();
    }

    public SessionMetric Create(SessionMetric sessionMetric)
    {
        _scoutingDbContext.SessionMetrics.Add(sessionMetric);
        _scoutingDbContext.SaveChanges();
        return sessionMetric;
    }

    public SessionMetric Update(SessionMetric sessionMetric)
    {
        _scoutingDbContext.SessionMetrics.Update(sessionMetric);
        _scoutingDbContext.SaveChanges();
        return sessionMetric;
    }

    public bool Delete(int id)
    {
        var sessionMetric = _scoutingDbContext.SessionMetrics.Find(id);
        if (sessionMetric == null) return false;

        _scoutingDbContext.SessionMetrics.Remove(sessionMetric);
        _scoutingDbContext.SaveChanges();
        return true;
    }
}
