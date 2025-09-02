using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface IPositionRepository
{
    Position? GetById(int id);
    IEnumerable<Position> GetAll();
    Position? GetByName(string name);
    Position Create(Position position);
    Position Update(Position position);
    bool Delete(int id);
}
