using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface IPlayerRepository
{
    Player? GetById(int id);
    IEnumerable<Player> GetAll();
    IEnumerable<Player> GetByNationalityId(int nationalityId);
    IEnumerable<Player> GetByPositionId(int positionId);
    Player Create(Player player);
    Player Update(Player player);
    bool Delete(int id);
}
