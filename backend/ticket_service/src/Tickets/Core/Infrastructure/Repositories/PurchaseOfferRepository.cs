using Microsoft.EntityFrameworkCore;
using ticket_service.src.Tickets.Core.Infrastructure;
using ticket_service.src.Tickets.Core.Application.Interfaces;
using ticket_service.src.Tickets.Core.Domain.Entities;

namespace ticket_service.src.Tickets.Core.Infrastructure.Repositories;

public class PurchaseOfferRepository : IPurchaseOfferRepository
{
    private readonly TicketDbContext _ticketDbContext;

    public PurchaseOfferRepository(TicketDbContext context)
    {
        _ticketDbContext = context;
    }

    public IEnumerable<PurchaseOffer> GetAll()
    {
        return _ticketDbContext.PurchaseOffers
            .Include(po => po.IdSeatNavigation)
            .ThenInclude(s => s.IdZoneNavigation)
            .ToList();
    }

    public PurchaseOffer? GetById(int id)
    {
        return _ticketDbContext.PurchaseOffers
            .Include(po => po.IdSeatNavigation)
            .ThenInclude(s => s.IdZoneNavigation)
            .FirstOrDefault(po => po.IdPurchaseOffer == id);
    }

    public IEnumerable<PurchaseOffer> GetByType(string type)
    {
        return _ticketDbContext.PurchaseOffers
            .Include(po => po.IdSeatNavigation)
            .ThenInclude(s => s.IdZoneNavigation)
            .Where(po => po.Type == type)
            .ToList();
    }

    public IEnumerable<PurchaseOffer> GetEnabledByType(string type)
    {
        return _ticketDbContext.PurchaseOffers
            .Include(po => po.IdSeatNavigation)
            .ThenInclude(s => s.IdZoneNavigation)
            .Where(po => po.Type == type && po.Status == "enabled")
            .ToList();
    }

    public PurchaseOffer Create(PurchaseOffer purchaseOffer)
    {
        _ticketDbContext.PurchaseOffers.Add(purchaseOffer);
        _ticketDbContext.SaveChanges();
        return purchaseOffer;
    }

    public PurchaseOffer Update(PurchaseOffer purchaseOffer)
    {
        _ticketDbContext.PurchaseOffers.Update(purchaseOffer);
        _ticketDbContext.SaveChanges();
        return purchaseOffer;
    }

    public void Delete(int id)
    {
        var purchaseOffer = _ticketDbContext.PurchaseOffers.Find(id);
        if (purchaseOffer != null)
        {
            _ticketDbContext.PurchaseOffers.Remove(purchaseOffer);
            _ticketDbContext.SaveChanges();
        }
    }

    public void UpdateStatus(int id, string status)
    {
        var purchaseOffer = _ticketDbContext.PurchaseOffers.Find(id);
        if (purchaseOffer != null)
        {
            purchaseOffer.Status = status;
            _ticketDbContext.SaveChanges();
        }
    }

    public IEnumerable<PurchaseOffer> GetSeasonTicketsBySeat(int seatId, int seasonId)
    {
        return _ticketDbContext.PurchaseOffers
            .Include(po => po.IdSeatNavigation)
            .Where(po => po.IdSeat == seatId && po.Type == "season ticket")
            .Join(_ticketDbContext.SeasonTickets,
                po => po.IdPurchaseOffer,
                st => st.IdPurchaseOffer,
                (po, st) => new { PurchaseOffer = po, SeasonTicket = st })
            .Where(x => x.SeasonTicket.IdSeason == seasonId)
            .Select(x => x.PurchaseOffer)
            .ToList();
    }

    public IEnumerable<PurchaseOffer> GetIndividualTicketsBySeat(int seatId)
    {
        return _ticketDbContext.PurchaseOffers
            .Include(po => po.IdSeatNavigation)
            .Where(po => po.IdSeat == seatId && po.Type == "individual ticket")
            .ToList();
    }

    public IEnumerable<PurchaseOffer> GetPurchaseHistoryByUserId(int userId)
    {
        return _ticketDbContext.PurchaseOffers
            .Include(po => po.IdSeatNavigation)
            .ThenInclude(s => s.IdZoneNavigation)
            .Include(po => po.CartItems)
            .ThenInclude(ci => ci.IdCartNavigation)
            .Where(po => po.CartItems.Any(ci => ci.IdCartNavigation.IdUser == userId) && po.Status == "bought")
            .ToList();
    }

    public IEnumerable<PurchaseOffer> GetExistingSeasonTicketsByZoneAndSeason(int zoneId, int seasonId)
    {
        return _ticketDbContext.PurchaseOffers
            .Include(po => po.IdSeatNavigation)
            .Include(po => po.CartItems)
            .ThenInclude(ci => ci.IdCartNavigation)
            .Where(po => po.Type == "season ticket" 
                      && po.IdSeatNavigation.IdZone == zoneId
                      && po.Status == "bought")
            .Join(_ticketDbContext.SeasonTickets,
                po => po.IdPurchaseOffer,
                st => st.IdPurchaseOffer,
                (po, st) => new { PurchaseOffer = po, SeasonTicket = st })
            .Where(x => x.SeasonTicket.IdSeason == seasonId)
            .Select(x => x.PurchaseOffer)
            .ToList();
    }
}
