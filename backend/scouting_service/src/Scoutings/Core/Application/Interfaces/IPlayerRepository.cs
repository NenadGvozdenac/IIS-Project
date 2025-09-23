using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface IPlayerRepository
{
    Player? GetById(int id);
    Player Create(Player player);
    Player Update(Player player);
    void Delete(int id);
    IEnumerable<Player> GetAll();
    IEnumerable<Player> GetByNationality(int nationalityId);
    IEnumerable<Player> GetByPosition(int positionId);
}
