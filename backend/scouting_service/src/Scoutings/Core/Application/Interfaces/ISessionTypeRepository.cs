using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface ISessionTypeRepository
{
    SessionType? GetById(int id);
    SessionType Create(SessionType sessionType);
    SessionType Update(SessionType sessionType);
    void Delete(int id);
    IEnumerable<SessionType> GetAll();
}
