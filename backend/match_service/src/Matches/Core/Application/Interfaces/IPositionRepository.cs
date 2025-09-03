using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Application.Interfaces
{
    public interface IPositionRepository
    {
        IEnumerable<Position> GetAll();
        Position? GetById(int id);
        bool Exists(int id);
    }
}
