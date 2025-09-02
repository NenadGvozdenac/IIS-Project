using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface IPhysicalMetricRepository
{
    PhysicalMetric? GetById(int id);
    IEnumerable<PhysicalMetric> GetAll();
    IEnumerable<PhysicalMetric> GetByPlayerId(int playerId);
    PhysicalMetric Create(PhysicalMetric physicalMetric);
    PhysicalMetric Update(PhysicalMetric physicalMetric);
    bool Delete(int id);
}
