using Microsoft.EntityFrameworkCore;
using ticket_service.src.Tickets.Core.Infrastructure;
using ticket_service.src.Tickets.Core.Application.Interfaces;
using ticket_service.src.Tickets.Core.Domain.Entities;

namespace ticket_service.src.Tickets.Core.Infrastructure.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly TicketDbContext _ticketDbContext;

    public MatchRepository(TicketDbContext context)
    {
        _ticketDbContext = context;
    }

    public IEnumerable<Match> GetAll()
    {
        return _ticketDbContext.Matches
            .Include(m => m.IdCompetitionNavigation)
            .Include(m => m.IdSeasonNavigation)
            .Include(m => m.IdTeamNavigation)
            .ToList();
    }

    public Match? GetById(int id)
    {
        return _ticketDbContext.Matches
            .Include(m => m.IdCompetitionNavigation)
            .Include(m => m.IdSeasonNavigation)
            .Include(m => m.IdTeamNavigation)
            .FirstOrDefault(m => m.IdMatch == id);
    }

    public IEnumerable<Match> GetMatchesInOurHall()
    {
        return _ticketDbContext.Matches
            .Include(m => m.IdCompetitionNavigation)
            .Include(m => m.IdSeasonNavigation)
            .Include(m => m.IdTeamNavigation)
            .Where(m => m.IsInOurHall)
            .ToList();
    }

    public IEnumerable<Match> GetUpcomingMatchesWithinDays(int days)
    {
        var currentDate = DateTime.UtcNow;
        var endDate = currentDate.AddDays(days);
        
        return _ticketDbContext.Matches
            .Include(m => m.IdCompetitionNavigation)
            .Include(m => m.IdSeasonNavigation)
            .Include(m => m.IdTeamNavigation)
            .Where(m => m.ScheduledAt >= currentDate && m.ScheduledAt <= endDate)
            .OrderBy(m => m.ScheduledAt)
            .ToList();
    }

    public void Update(Match match)
    {
        _ticketDbContext.Matches.Update(match);
        _ticketDbContext.SaveChanges();
    }
}
