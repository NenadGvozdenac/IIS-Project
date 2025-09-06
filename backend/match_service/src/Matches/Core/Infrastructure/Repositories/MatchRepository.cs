using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Domain.Entities;
using match_service.src.Matches.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace match_service.src.Matches.Core.Infrastructure.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly MatchDbContext _context;

    public MatchRepository(MatchDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Match> GetAll()
    {
        return _context.Matches
            .Include(m => m.IdTeamNavigation)
            .Include(m => m.IdSeasonNavigation)
            .Include(m => m.IdCompetitionNavigation)
            .Include(m => m.MatchTracking)
            .OrderBy(m => m.ScheduledAt)
            .ToList();
    }

    public Match? GetById(int id)
    {
        return _context.Matches
            .Include(m => m.IdTeamNavigation)
            .Include(m => m.IdSeasonNavigation)
            .Include(m => m.IdCompetitionNavigation)
            .Include(m => m.MatchTracking)
            .FirstOrDefault(m => m.IdMatch == id);
    }

    public Match Create(Match match)
    {
        _context.Matches.Add(match);
        _context.SaveChanges();
        return match;
    }

    public Match? Update(Match match)
    {
        var existingMatch = _context.Matches.FirstOrDefault(m => m.IdMatch == match.IdMatch);
        if (existingMatch == null)
            return null;

        existingMatch.Name = match.Name;
        existingMatch.ScheduledAt = match.ScheduledAt;
        existingMatch.Type = match.Type;
        existingMatch.State = match.State;
        existingMatch.City = match.City;
        existingMatch.Hall = match.Hall;
        existingMatch.IsInOurHall = match.IsInOurHall;
        existingMatch.IdCompetition = match.IdCompetition;
        existingMatch.IdSeason = match.IdSeason;
        existingMatch.IdTeam = match.IdTeam;

        _context.SaveChanges();
        return existingMatch;
    }

    public bool Delete(int id)
    {
        var match = _context.Matches.FirstOrDefault(m => m.IdMatch == id);
        if (match == null)
            return false;

        _context.Matches.Remove(match);
        _context.SaveChanges();
        return true;
    }

    public bool Exists(int id)
    {
        return _context.Matches.Any(m => m.IdMatch == id);
    }
}
