using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Application.Interfaces
{
    public interface IPlayerRepository
    {
        Player? GetById(int id);
        Player Create(Player player);
        bool Exists(int id);
    }
}
