using Microsoft.EntityFrameworkCore;
using ticket_service.src.Tickets.Core.Infrastructure;
using ticket_service.src.Tickets.Core.Application.Interfaces;
using ticket_service.src.Tickets.Core.Domain.Entities;

namespace ticket_service.src.Tickets.Core.Infrastructure.Repositories;

public class IndividualTicketRepository : IIndividualTicketRepository
{
    private readonly TicketDbContext _ticketDbContext;

    public IndividualTicketRepository(TicketDbContext context)
    {
        _ticketDbContext = context;
    }

    public IndividualTicket? GetByPurchaseOfferId(int purchaseOfferId)
    {
        return _ticketDbContext.IndividualTickets
            .Include(it => it.IdMatchNavigation)
            .FirstOrDefault(it => it.IdPurchaseOffer == purchaseOfferId);
    }

    public IEnumerable<IndividualTicket> GetByMatch(int matchId)
    {
        return _ticketDbContext.IndividualTickets
            .Include(it => it.IdMatchNavigation)
            .Where(it => it.IdMatch == matchId)
            .ToList();
    }

    public IndividualTicket Create(IndividualTicket individualTicket)
    {
        _ticketDbContext.IndividualTickets.Add(individualTicket);
        _ticketDbContext.SaveChanges();
        return individualTicket;
    }

    public IndividualTicket Update(IndividualTicket individualTicket)
    {
        _ticketDbContext.IndividualTickets.Update(individualTicket);
        _ticketDbContext.SaveChanges();
        return individualTicket;
    }

    public void Delete(int purchaseOfferId)
    {
        var individualTicket = _ticketDbContext.IndividualTickets.Find(purchaseOfferId);
        if (individualTicket != null)
        {
            _ticketDbContext.IndividualTickets.Remove(individualTicket);
            _ticketDbContext.SaveChanges();
        }
    }

    public IEnumerable<IndividualTicket> GetAvailableByMatch(int matchId)
    {
        return _ticketDbContext.IndividualTickets
            .Include(it => it.IdMatchNavigation)
            .Join(_ticketDbContext.PurchaseOffers,
                it => it.IdPurchaseOffer,
                po => po.IdPurchaseOffer,
                (it, po) => new { IndividualTicket = it, PurchaseOffer = po })
            .Where(x => x.IndividualTicket.IdMatch == matchId && x.PurchaseOffer.Status == "enabled")
            .Select(x => x.IndividualTicket)
            .ToList();
    }
}
