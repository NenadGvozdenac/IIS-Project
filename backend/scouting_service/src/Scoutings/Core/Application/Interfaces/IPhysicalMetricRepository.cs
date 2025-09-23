using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface IPhysicalMetricRepository
{
    PhysicalMetric? GetById(int id);
    PhysicalMetric Create(PhysicalMetric physicalMetric);
    PhysicalMetric Update(PhysicalMetric physicalMetric);
    void Delete(int id);
    IEnumerable<PhysicalMetric> GetAll();
    IEnumerable<PhysicalMetric> GetByPlayer(int playerId);
}
