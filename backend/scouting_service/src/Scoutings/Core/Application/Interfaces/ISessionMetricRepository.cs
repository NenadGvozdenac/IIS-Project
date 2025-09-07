using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface ISessionMetricRepository
{
    SessionMetric? GetById(int id);
    SessionMetric Create(SessionMetric sessionMetric);
    SessionMetric Update(SessionMetric sessionMetric);
    void Delete(int id);
    IEnumerable<SessionMetric> GetAll();
    IEnumerable<SessionMetric> GetBySession(int sessionId);
    IEnumerable<SessionMetric> GetByMetric(int metricId);
}
