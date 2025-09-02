using Microsoft.EntityFrameworkCore;
using ticket_service.src.Tickets.Core.Application.Interfaces;
using ticket_service.src.Tickets.Core.Domain.Entities;
using ticket_service.src.Tickets.Core.Infrastructure;

namespace ticket_service.src.Tickets.Core.Infrastructure.Repositories;

public class CompetitionRepository : ICompetitionRepository
{
    private readonly TicketDbContext _ticketDbContext;

    public CompetitionRepository(TicketDbContext context)
    {
        _ticketDbContext = context;
    }

    public IEnumerable<Competition> GetAll()
    {
        return _ticketDbContext.Competitions
            .Include(c => c.Matches)
            .OrderBy(c => c.StartedAt)
            .ToList();
    }

    public Competition? GetById(int id)
    {
        return _ticketDbContext.Competitions
            .Include(c => c.Matches)
            .FirstOrDefault(c => c.IdCompetition == id);
    }

    public Competition Create(Competition competition)
    {
        _ticketDbContext.Competitions.Add(competition);
        _ticketDbContext.SaveChanges();
        return competition;
    }

    public Competition Update(Competition competition)
    {
        _ticketDbContext.Competitions.Update(competition);
        _ticketDbContext.SaveChanges();
        return competition;
    }

    public void Delete(int id)
    {
        var competition = _ticketDbContext.Competitions.Find(id);
        if (competition != null)
        {
            _ticketDbContext.Competitions.Remove(competition);
            _ticketDbContext.SaveChanges();
        }
    }

    public bool ExistsByName(string name)
    {
        return _ticketDbContext.Competitions
            .Any(c => c.Name.ToLower() == name.ToLower());
    }

    public bool ExistsByNameExcludingId(string name, int id)
    {
        return _ticketDbContext.Competitions
            .Any(c => c.Name.ToLower() == name.ToLower() && c.IdCompetition != id);
    }

    public IEnumerable<Competition> GetActiveCompetitions()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return _ticketDbContext.Competitions
            .Include(c => c.Matches)
            .Where(c => c.StartedAt <= today && (c.EndedAt == null || c.EndedAt >= today))
            .OrderBy(c => c.StartedAt)
            .ToList();
    }

    public IEnumerable<Competition> GetCompetitionsByDateRange(DateOnly startDate, DateOnly endDate)
    {
        return _ticketDbContext.Competitions
            .Include(c => c.Matches)
            .Where(c => c.StartedAt >= startDate && c.StartedAt <= endDate)
            .OrderBy(c => c.StartedAt)
            .ToList();
    }
}
