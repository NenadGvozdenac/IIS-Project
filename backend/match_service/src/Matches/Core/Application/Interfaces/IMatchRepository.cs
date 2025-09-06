using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Application.Interfaces;

public interface IMatchRepository
{
    IEnumerable<Match> GetAll();
    Match? GetById(int id);
    Match Create(Match match);
    Match? Update(Match match);
    bool Delete(int id);
    bool Exists(int id);
}
