using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

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
        return _context.SessionMetrics
            .Include(sm => sm.IdSessionNavigation)
            .Include(sm => sm.IdMetricsNavigation)
                .ThenInclude(m => m.IdMetricTypeNavigation)
            .Include(sm => sm.IdMetricsNavigation)
                .ThenInclude(m => m.IdUserNavigation)
            .ToList();
    }

    public SessionMetric? GetById(int sessionId, int metricId)
    {
        return _context.SessionMetrics
            .Include(sm => sm.IdSessionNavigation)
            .Include(sm => sm.IdMetricsNavigation)
                .ThenInclude(m => m.IdMetricTypeNavigation)
            .Include(sm => sm.IdMetricsNavigation)
                .ThenInclude(m => m.IdUserNavigation)
            .FirstOrDefault(sm => sm.IdSession == sessionId && sm.IdMetrics == metricId);
    }

    public SessionMetric? GetBySessionAndMetric(int sessionId, int metricId)
    {
        return _context.SessionMetrics
            .Include(sm => sm.IdSessionNavigation)
            .Include(sm => sm.IdMetricsNavigation)
                .ThenInclude(m => m.IdMetricTypeNavigation)
            .Include(sm => sm.IdMetricsNavigation)
                .ThenInclude(m => m.IdUserNavigation)
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

    public void Delete(int sessionId, int metricId)
    {
        var sessionMetric = _context.SessionMetrics
            .FirstOrDefault(sm => sm.IdSession == sessionId && sm.IdMetrics == metricId);
        if (sessionMetric != null)
        {
            _context.SessionMetrics.Remove(sessionMetric);
            _context.SaveChanges();
        }
    }

    public IEnumerable<SessionMetric> GetBySession(int sessionId)
    {
        return _context.SessionMetrics
            .Include(sm => sm.IdSessionNavigation)
            .Include(sm => sm.IdMetricsNavigation)
                .ThenInclude(m => m.IdMetricTypeNavigation)
            .Include(sm => sm.IdMetricsNavigation)
                .ThenInclude(m => m.IdUserNavigation)
            .Where(sm => sm.IdSession == sessionId)
            .ToList();
    }

    public IEnumerable<SessionMetric> GetByMetric(int metricId)
    {
        return _context.SessionMetrics
            .Include(sm => sm.IdSessionNavigation)
            .Include(sm => sm.IdMetricsNavigation)
                .ThenInclude(m => m.IdMetricTypeNavigation)
            .Include(sm => sm.IdMetricsNavigation)
                .ThenInclude(m => m.IdUserNavigation)
            .Where(sm => sm.IdMetrics == metricId)
            .ToList();
    }
}
