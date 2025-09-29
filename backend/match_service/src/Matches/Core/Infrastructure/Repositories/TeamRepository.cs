using match_service.src.Matches.Core.Infrastructure;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Infrastructure.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly MatchDbContext _matchDbContext;

    public TeamRepository(MatchDbContext context)
    {
        _matchDbContext = context;
    }

    public Team? GetById(int id)
    {
        return _matchDbContext.Teams.Find(id);
    }

    public IEnumerable<Team> GetAll()
    {
        return _matchDbContext.Teams.ToList();
    }

    public Team Create(Team team)
    {
        _matchDbContext.Teams.Add(team);
        _matchDbContext.SaveChanges();
        return team;
    }

    public Team? Update(Team team)
    {
        var existingTeam = _matchDbContext.Teams.Find(team.IdTeam);
        if (existingTeam == null)
            return null;

        existingTeam.Name = team.Name;
        existingTeam.State = team.State;
        existingTeam.City = team.City;
        existingTeam.Hall = team.Hall;
        existingTeam.FoundedDate = team.FoundedDate;
        existingTeam.Coach = team.Coach;
        existingTeam.PlayingStyle = team.PlayingStyle;
        existingTeam.KeyStrengths = team.KeyStrengths;
        existingTeam.KeyWeaknesses = team.KeyWeaknesses;

        _matchDbContext.SaveChanges();
        return existingTeam;
    }

    public bool Delete(int id)
    {
        var team = _matchDbContext.Teams.Find(id);
        if (team == null)
            return false;

        _matchDbContext.Teams.Remove(team);
        _matchDbContext.SaveChanges();
        return true;
    }

    public bool Exists(int id)
    {
        return _matchDbContext.Teams.Any(t => t.IdTeam == id);
    }
}