using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface IMetricTypeRepository
{
    MetricType? GetById(int id);
    IEnumerable<MetricType> GetAll();
    MetricType? GetByName(string name);
    MetricType Create(MetricType metricType);
    MetricType Update(MetricType metricType);
    bool Delete(int id);
}
