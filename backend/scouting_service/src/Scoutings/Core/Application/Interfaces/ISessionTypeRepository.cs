using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface ISessionTypeRepository
{
    SessionType? GetById(int id);
    IEnumerable<SessionType> GetAll();
    SessionType? GetByName(string name);
    SessionType Create(SessionType sessionType);
    SessionType Update(SessionType sessionType);
    bool Delete(int id);
}
