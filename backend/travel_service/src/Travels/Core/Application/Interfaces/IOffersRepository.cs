using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Application.Interfaces;

public interface IOffersRepository
{
    IEnumerable<Offer> GetAllOffers();
    IEnumerable<Offer> GetOffersByType(string type);
    IEnumerable<Offer> GetOffersByTypeAndMatch(string type, int idMatch);
    Offer? GetChosenOfferByTypeAndMatch(string type, int idMatch);
    Offer? GetOfferById(int idOffer);
    Offer CreateOffer(Offer offer);
    void UpdateOffer(Offer offer);
    void DeleteOffer(int idOffer);
}