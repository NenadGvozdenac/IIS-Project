using Microsoft.EntityFrameworkCore;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;
using ticket_service.src.Tickets.Core.Domain.Entities.Relational;

namespace ticket_service.src.Tickets.Core.Infrastructure.Repositories.Relational;

public class SeasonRepository : ISeasonRepository
{
    private readonly TicketDbContext _ticketDbContext;

    public SeasonRepository(TicketDbContext context)
    {
        _ticketDbContext = context;
    }

    public Season? GetById(int id)
    {
        return _ticketDbContext.Seasons
            .Include(s => s.Matches)
            .Include(s => s.SeasonTickets)
            .FirstOrDefault(s => s.IdSeason == id);
    }

    public IEnumerable<Season> GetAll()
    {
        return _ticketDbContext.Seasons
            .Include(s => s.Matches)
            .Include(s => s.SeasonTickets)
            .OrderBy(s => s.StartedAt)
            .ToList();
    }

    public Season Create(Season season)
    {
        _ticketDbContext.Seasons.Add(season);
        _ticketDbContext.SaveChanges();
        return season;
    }

    public Season Update(Season season)
    {
        _ticketDbContext.Seasons.Update(season);
        _ticketDbContext.SaveChanges();
        return season;
    }

    public bool Delete(int id)
    {
        var season = _ticketDbContext.Seasons.Find(id);
        if (season == null) return false;

        _ticketDbContext.Seasons.Remove(season);
        _ticketDbContext.SaveChanges();
        return true;
    }

    public bool ExistsByName(string name)
    {
        return _ticketDbContext.Seasons.Any(s => s.Name == name);
    }

    public bool ExistsByName(string name, int excludeId)
    {
        return _ticketDbContext.Seasons.Any(s => s.Name == name && s.IdSeason != excludeId);
    }

    public bool ReleaseSeasonCards(int seasonId)
    {
        var season = _ticketDbContext.Seasons.Find(seasonId);
        if (season == null) return false;

        season.TicketsForSale = true;
        season.TicketsWentOnSale = DateTime.UtcNow;

        _ticketDbContext.SaveChanges();
        return true;
    }
}
