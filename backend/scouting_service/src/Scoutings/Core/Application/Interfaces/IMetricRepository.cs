using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface IMetricRepository
{
    Metric? GetById(int id);
    Metric Create(Metric metric);
    Metric Update(Metric metric);
    void Delete(int id);
    IEnumerable<Metric> GetAll();
    IEnumerable<Metric> GetByUser(int userId);
    IEnumerable<Metric> GetByType(int typeId);
    IEnumerable<Metric> GetPermanent();
}
