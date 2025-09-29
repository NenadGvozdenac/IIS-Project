using ticket_service.src.Tickets.Core.Domain.Entities.Relational;

namespace ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

public interface IPurchaseOfferRepository
{
    IEnumerable<PurchaseOffer> GetAll();
    PurchaseOffer? GetById(int id);
    IEnumerable<PurchaseOffer> GetByType(string type);
    IEnumerable<PurchaseOffer> GetEnabledByType(string type);
    PurchaseOffer Create(PurchaseOffer purchaseOffer);
    PurchaseOffer Update(PurchaseOffer purchaseOffer);
    void Delete(int id);
    void UpdateStatus(int id, string status);
    IEnumerable<PurchaseOffer> GetSeasonTicketsBySeat(int seatId, int seasonId);
    IEnumerable<PurchaseOffer> GetIndividualTicketsBySeat(int seatId);
    IEnumerable<PurchaseOffer> GetPurchaseHistoryByUserId(int userId);
    IEnumerable<PurchaseOffer> GetExistingSeasonTicketsByZoneAndSeason(int zoneId, int seasonId);
}
