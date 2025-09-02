using Microsoft.EntityFrameworkCore;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;

namespace scouting_service.src.Scoutings.Core.Infrastructure.Repositories;

public class PhysicalMetricRepository : IPhysicalMetricRepository
{
    private readonly ScoutingDbContext _scoutingDbContext;

    public PhysicalMetricRepository(ScoutingDbContext context)
    {
        _scoutingDbContext = context;
    }

    public PhysicalMetric? GetById(int id)
    {
        return _scoutingDbContext.PhysicalMetrics
            .Include(pm => pm.IdPlayerNavigation)
            .FirstOrDefault(pm => pm.IdPhysicalMetrics == id);
    }

    public IEnumerable<PhysicalMetric> GetAll()
    {
        return _scoutingDbContext.PhysicalMetrics
            .Include(pm => pm.IdPlayerNavigation)
            .ToList();
    }

    public IEnumerable<PhysicalMetric> GetByPlayerId(int playerId)
    {
        return _scoutingDbContext.PhysicalMetrics
            .Include(pm => pm.IdPlayerNavigation)
            .Where(pm => pm.IdPlayer == playerId)
            .ToList();
    }

    public PhysicalMetric Create(PhysicalMetric physicalMetric)
    {
        _scoutingDbContext.PhysicalMetrics.Add(physicalMetric);
        _scoutingDbContext.SaveChanges();
        return physicalMetric;
    }

    public PhysicalMetric Update(PhysicalMetric physicalMetric)
    {
        _scoutingDbContext.PhysicalMetrics.Update(physicalMetric);
        _scoutingDbContext.SaveChanges();
        return physicalMetric;
    }

    public bool Delete(int id)
    {
        var physicalMetric = _scoutingDbContext.PhysicalMetrics.Find(id);
        if (physicalMetric == null) return false;

        _scoutingDbContext.PhysicalMetrics.Remove(physicalMetric);
        _scoutingDbContext.SaveChanges();
        return true;
    }
}
