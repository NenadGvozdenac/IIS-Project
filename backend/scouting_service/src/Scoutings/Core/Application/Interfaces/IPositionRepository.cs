using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface IPositionRepository
{
    Position? GetById(int id);
    Position Create(Position position);
    Position Update(Position position);
    void Delete(int id);
    IEnumerable<Position> GetAll();
}
