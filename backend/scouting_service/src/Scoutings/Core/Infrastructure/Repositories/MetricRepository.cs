using Microsoft.EntityFrameworkCore;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;

namespace scouting_service.src.Scoutings.Core.Infrastructure.Repositories;

public class MetricRepository : IMetricRepository
{
    private readonly ScoutingDbContext _scoutingDbContext;

    public MetricRepository(ScoutingDbContext context)
    {
        _scoutingDbContext = context;
    }

    public Metric? GetById(int id)
    {
        return _scoutingDbContext.Metrics
            .Include(m => m.IdMetricTypeNavigation)
            .Include(m => m.SessionMetrics)
            .FirstOrDefault(m => m.IdMetrics == id);
    }

    public IEnumerable<Metric> GetAll()
    {
        return _scoutingDbContext.Metrics
            .Include(m => m.IdMetricTypeNavigation)
            .Include(m => m.SessionMetrics)
            .ToList();
    }

    public IEnumerable<Metric> GetByUserId(int userId)
    {
        return _scoutingDbContext.Metrics
            .Include(m => m.IdMetricTypeNavigation)
            .Include(m => m.SessionMetrics)
            .Where(m => m.IdUser == userId)
            .ToList();
    }

    public IEnumerable<Metric> GetByMetricType(int metricTypeId)
    {
        return _scoutingDbContext.Metrics
            .Include(m => m.IdMetricTypeNavigation)
            .Include(m => m.SessionMetrics)
            .Where(m => m.IdMetricType == metricTypeId)
            .ToList();
    }

    public Metric Create(Metric metric)
    {
        _scoutingDbContext.Metrics.Add(metric);
        _scoutingDbContext.SaveChanges();
        return metric;
    }

    public Metric Update(Metric metric)
    {
        _scoutingDbContext.Metrics.Update(metric);
        _scoutingDbContext.SaveChanges();
        return metric;
    }

    public bool Delete(int id)
    {
        var metric = _scoutingDbContext.Metrics.Find(id);
        if (metric == null) return false;

        _scoutingDbContext.Metrics.Remove(metric);
        _scoutingDbContext.SaveChanges();
        return true;
    }
}
