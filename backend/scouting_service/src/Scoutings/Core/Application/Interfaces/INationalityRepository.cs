using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface INationalityRepository
{
    Nationality? GetById(int id);
    IEnumerable<Nationality> GetAll();
    Nationality? GetByName(string name);
    Nationality Create(Nationality nationality);
    Nationality Update(Nationality nationality);
    bool Delete(int id);
}
