using Microsoft.EntityFrameworkCore;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;

namespace scouting_service.src.Scoutings.Core.Infrastructure.Repositories;

public class MetricTypeRepository : IMetricTypeRepository
{
    private readonly ScoutingDbContext _scoutingDbContext;

    public MetricTypeRepository(ScoutingDbContext context)
    {
        _scoutingDbContext = context;
    }

    public MetricType? GetById(int id)
    {
        return _scoutingDbContext.MetricTypes
            .Include(mt => mt.Metrics)
            .FirstOrDefault(mt => mt.IdMetricType == id);
    }

    public IEnumerable<MetricType> GetAll()
    {
        return _scoutingDbContext.MetricTypes
            .Include(mt => mt.Metrics)
            .ToList();
    }

    public MetricType? GetByName(string name)
    {
        return _scoutingDbContext.MetricTypes
            .Include(mt => mt.Metrics)
            .FirstOrDefault(mt => mt.Name == name);
    }

    public MetricType Create(MetricType metricType)
    {
        _scoutingDbContext.MetricTypes.Add(metricType);
        _scoutingDbContext.SaveChanges();
        return metricType;
    }

    public MetricType Update(MetricType metricType)
    {
        _scoutingDbContext.MetricTypes.Update(metricType);
        _scoutingDbContext.SaveChanges();
        return metricType;
    }

    public bool Delete(int id)
    {
        var metricType = _scoutingDbContext.MetricTypes.Find(id);
        if (metricType == null) return false;

        _scoutingDbContext.MetricTypes.Remove(metricType);
        _scoutingDbContext.SaveChanges();
        return true;
    }
}
