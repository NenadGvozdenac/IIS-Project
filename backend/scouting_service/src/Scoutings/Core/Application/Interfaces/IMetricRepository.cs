using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface IMetricRepository
{
    Metric? GetById(int id);
    IEnumerable<Metric> GetAll();
    IEnumerable<Metric> GetByUserId(int userId);
    IEnumerable<Metric> GetByMetricType(int metricTypeId);
    Metric Create(Metric metric);
    Metric Update(Metric metric);
    bool Delete(int id);
}
