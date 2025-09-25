using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface ISessionStatusRepository
{
    SessionStatus? GetById(int id);
    SessionStatus Create(SessionStatus sessionStatus);
    SessionStatus Update(SessionStatus sessionStatus);
    void Delete(int id);
    IEnumerable<SessionStatus> GetAll();
}
