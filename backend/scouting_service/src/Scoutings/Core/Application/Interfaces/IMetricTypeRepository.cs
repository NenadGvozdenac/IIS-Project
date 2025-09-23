using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface IMetricTypeRepository
{
    MetricType? GetById(int id);
    MetricType Create(MetricType metricType);
    MetricType Update(MetricType metricType);
    void Delete(int id);
    IEnumerable<MetricType> GetAll();
}
