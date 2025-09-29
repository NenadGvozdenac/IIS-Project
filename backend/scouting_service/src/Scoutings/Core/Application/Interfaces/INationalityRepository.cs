using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Interfaces;

public interface INationalityRepository
{
    Nationality? GetById(int id);
    Nationality Create(Nationality nationality);
    Nationality Update(Nationality nationality);
    void Delete(int id);
    IEnumerable<Nationality> GetAll();
}
