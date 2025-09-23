using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;

namespace scouting_service.src.Scoutings.Core.Infrastructure.Repositories;

public class MetricTypeRepository : IMetricTypeRepository
{
    private readonly ScoutingDbContext _context;

    public MetricTypeRepository(ScoutingDbContext context)
    {
        _context = context;
    }

    public IEnumerable<MetricType> GetAll()
    {
        return _context.MetricTypes.ToList();
    }

    public MetricType? GetById(int id)
    {
        return _context.MetricTypes.Find(id);
    }

    public MetricType Create(MetricType metricType)
    {
        _context.MetricTypes.Add(metricType);
        _context.SaveChanges();
        return metricType;
    }

    public MetricType Update(MetricType metricType)
    {
        _context.MetricTypes.Update(metricType);
        _context.SaveChanges();
        return metricType;
    }

    public void Delete(int id)
    {
        var metricType = _context.MetricTypes.Find(id);
        if (metricType != null)
        {
            _context.MetricTypes.Remove(metricType);
            _context.SaveChanges();
        }
    }
}
