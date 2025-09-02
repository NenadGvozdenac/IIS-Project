using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface ISessionStatusRepository
{
    SessionStatus? GetById(int id);
    IEnumerable<SessionStatus> GetAll();
    SessionStatus? GetByName(string name);
    SessionStatus Create(SessionStatus sessionStatus);
    SessionStatus Update(SessionStatus sessionStatus);
    bool Delete(int id);
}
