using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface ISeasonRepository
{
    Season? GetById(int id);
    IEnumerable<Season> GetAll();
    Season? GetByName(string name);
    Season Create(Season season);
    Season Update(Season season);
    bool Delete(int id);
}
