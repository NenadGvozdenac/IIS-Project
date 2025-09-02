using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface ISessionMetricRepository
{
    SessionMetric? GetById(int id);
    IEnumerable<SessionMetric> GetAll();
    IEnumerable<SessionMetric> GetBySessionId(int sessionId);
    IEnumerable<SessionMetric> GetByMetricId(int metricId);
    SessionMetric Create(SessionMetric sessionMetric);
    SessionMetric Update(SessionMetric sessionMetric);
    bool Delete(int id);
}
