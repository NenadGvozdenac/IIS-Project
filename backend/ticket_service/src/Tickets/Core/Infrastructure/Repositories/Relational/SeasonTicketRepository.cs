using Microsoft.EntityFrameworkCore;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;
using ticket_service.src.Tickets.Core.Domain.Entities.Relational;

namespace ticket_service.src.Tickets.Core.Infrastructure.Repositories.Relational;

public class SeasonTicketRepository : ISeasonTicketRepository
{
    private readonly TicketDbContext _ticketDbContext;

    public SeasonTicketRepository(TicketDbContext context)
    {
        _ticketDbContext = context;
    }

    public SeasonTicket? GetByPurchaseOfferId(int purchaseOfferId)
    {
        return _ticketDbContext.SeasonTickets
            .Include(st => st.IdSeasonNavigation)
            .FirstOrDefault(st => st.IdPurchaseOffer == purchaseOfferId);
    }

    public IEnumerable<SeasonTicket> GetBySeason(int seasonId)
    {
        return _ticketDbContext.SeasonTickets
            .Include(st => st.IdSeasonNavigation)
            .Where(st => st.IdSeason == seasonId)
            .ToList();
    }

    public SeasonTicket Create(SeasonTicket seasonTicket)
    {
        _ticketDbContext.SeasonTickets.Add(seasonTicket);
        _ticketDbContext.SaveChanges();
        return seasonTicket;
    }

    public SeasonTicket Update(SeasonTicket seasonTicket)
    {
        _ticketDbContext.SeasonTickets.Update(seasonTicket);
        _ticketDbContext.SaveChanges();
        return seasonTicket;
    }

    public void Delete(int purchaseOfferId)
    {
        var seasonTicket = _ticketDbContext.SeasonTickets.Find(purchaseOfferId);
        if (seasonTicket != null)
        {
            _ticketDbContext.SeasonTickets.Remove(seasonTicket);
            _ticketDbContext.SaveChanges();
        }
    }
}
