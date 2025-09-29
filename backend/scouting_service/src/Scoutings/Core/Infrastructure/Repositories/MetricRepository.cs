using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace scouting_service.src.Scoutings.Core.Infrastructure.Repositories;

public class MetricRepository : IMetricRepository
{
    private readonly ScoutingDbContext _context;

    public MetricRepository(ScoutingDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Metric> GetAll()
    {
        return _context.Metrics
            .Include(m => m.IdUserNavigation)
            .Include(m => m.IdMetricTypeNavigation)
            .ToList();
    }

    public Metric? GetById(int id)
    {
        return _context.Metrics
            .Include(m => m.IdUserNavigation)
            .Include(m => m.IdMetricTypeNavigation)
            .FirstOrDefault(m => m.IdMetrics == id);
    }

    public Metric Create(Metric metric)
    {
        _context.Metrics.Add(metric);
        _context.SaveChanges();
        return metric;
    }

    public Metric Update(Metric metric)
    {
        _context.Metrics.Update(metric);
        _context.SaveChanges();
        return metric;
    }

    public void Delete(int id)
    {
        var metric = _context.Metrics.Find(id);
        if (metric != null)
        {
            _context.Metrics.Remove(metric);
            _context.SaveChanges();
        }
    }

    public IEnumerable<Metric> GetByUser(int userId)
    {
        return _context.Metrics
            .Include(m => m.IdUserNavigation)
            .Include(m => m.IdMetricTypeNavigation)
            .Where(m => m.IdUser == userId)
            .ToList();
    }

    public IEnumerable<Metric> GetByType(int typeId)
    {
        return _context.Metrics
            .Include(m => m.IdUserNavigation)
            .Include(m => m.IdMetricTypeNavigation)
            .Where(m => m.IdMetricType == typeId)
            .ToList();
    }

    public IEnumerable<Metric> GetPermanent()
    {
        return _context.Metrics
            .Include(m => m.IdUserNavigation)
            .Include(m => m.IdMetricTypeNavigation)
            .Where(m => m.IsPermanent == 1)
            .ToList();
    }
}
