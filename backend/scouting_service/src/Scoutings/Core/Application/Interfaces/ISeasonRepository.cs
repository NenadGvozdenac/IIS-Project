using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface ISeasonRepository
{
    Season? GetById(int id);
    Season Create(Season season);
    Season Update(Season season);
    void Delete(int id);
    IEnumerable<Season> GetAll();
}
