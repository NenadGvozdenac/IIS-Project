using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface ISessionRepository
{
    Session? GetById(int id);
    Session Create(Session session);
    Session Update(Session session);
    void Delete(int id);
    IEnumerable<Session> GetAll();
    IEnumerable<Session> GetByPlayer(int playerId);
    IEnumerable<Session> GetByUser(int userId);
    IEnumerable<Session> GetByStatus(int statusId);
    IEnumerable<Session> GetByType(int typeId);
}
