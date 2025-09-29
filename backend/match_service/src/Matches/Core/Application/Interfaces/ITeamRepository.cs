using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Application.Interfaces;

public interface ITeamRepository
{
    Team? GetById(int id);
    IEnumerable<Team> GetAll();
    Team Create(Team team);
    Team? Update(Team team);
    bool Delete(int id);
    bool Exists(int id);
}