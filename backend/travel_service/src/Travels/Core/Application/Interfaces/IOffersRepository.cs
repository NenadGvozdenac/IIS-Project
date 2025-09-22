using travel_service.src.Travels.Core.Domain.Entities;
using travel_service.src.Travels.Core.Application.Commands.AutoSelectBestOffer;

namespace travel_service.src.Travels.Core.Application.Interfaces;

public interface IOffersRepository
{
    IEnumerable<Offer> GetAllOffers();
    IEnumerable<Offer> GetOffersByType(string type);
    IEnumerable<Offer> GetOffersByTypeAndMatch(string type, int idMatch);
    Offer? GetChosenOfferByTypeAndMatch(string type, int idMatch);
    Offer? GetOfferById(int idOffer);
    Offer? GetOfferByCompositeKey(int idOffer, int idAgency, int idRequest);
    Offer CreateOffer(Offer offer);
    void UpdateOfferStatus(int idOffer, int idAgency, int idRequest, bool chosen);
    Task<AutoSelectBestOfferResponse?> AutoSelectBestOffer(int matchId, string offerType, decimal weightPrice, decimal weightCapacity, decimal weightBenefits, decimal weightAgency);
}