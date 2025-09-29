using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Application.Interfaces;

public interface IMatchRepository
{
    Match? GetById(int id);
    IEnumerable<Match> GetAll();
    Match Create(Match match);
    Match Update(Match match);
    bool Delete(int id);
}