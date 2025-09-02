using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface ISessionRepository
{
    Session? GetById(int id);
    IEnumerable<Session> GetAll();
    IEnumerable<Session> GetByPlayerId(int playerId);
    IEnumerable<Session> GetByUserId(int userId);
    IEnumerable<Session> GetBySessionType(int sessionTypeId);
    IEnumerable<Session> GetBySessionStatus(int sessionStatusId);
    Session Create(Session session);
    Session Update(Session session);
    bool Delete(int id);
}
