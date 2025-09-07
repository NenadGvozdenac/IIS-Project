using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;

namespace scouting_service.src.Scoutings.Core.Infrastructure.Repositories;

public class PhysicalMetricRepository : IPhysicalMetricRepository
{
    private readonly ScoutingDbContext _context;

    public PhysicalMetricRepository(ScoutingDbContext context)
    {
        _context = context;
    }

    public IEnumerable<PhysicalMetric> GetAll()
    {
        return _context.PhysicalMetrics.ToList();
    }

    public PhysicalMetric? GetById(int id)
    {
        return _context.PhysicalMetrics.Find(id);
    }

    public PhysicalMetric Create(PhysicalMetric physicalMetric)
    {
        _context.PhysicalMetrics.Add(physicalMetric);
        _context.SaveChanges();
        return physicalMetric;
    }

    public PhysicalMetric Update(PhysicalMetric physicalMetric)
    {
        _context.PhysicalMetrics.Update(physicalMetric);
        _context.SaveChanges();
        return physicalMetric;
    }

    public void Delete(int id)
    {
        var physicalMetric = _context.PhysicalMetrics.Find(id);
        if (physicalMetric != null)
        {
            _context.PhysicalMetrics.Remove(physicalMetric);
            _context.SaveChanges();
        }
    }

    public IEnumerable<PhysicalMetric> GetByPlayer(int playerId)
    {
        return _context.PhysicalMetrics.Where(pm => pm.IdPlayer == playerId).ToList();
    }
}
