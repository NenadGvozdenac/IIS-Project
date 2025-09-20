using Microsoft.EntityFrameworkCore;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;
using travel_service.src.Travels.Core.Infrastructure;

namespace travel_service.src.Travels.Core.Infrastructure.Repositories;

public class OffersRepository : IOffersRepository
{
    private readonly TravelDbContext _travelDbContext;

    public OffersRepository(TravelDbContext context)
    {
        _travelDbContext = context;
    }

    public IEnumerable<Offer> GetAllOffers()
    {
        return _travelDbContext.Offers
            .Include(o => o.AccommodationOffer)
            .Include(o => o.TransportationOffer)
            .Include(o => o.IdMatchNavigation)
            .Include(o => o.UserIdUserNavigation)
            .ToList();
    }

    public IEnumerable<Offer> GetOffersByType(string type)
    {
        return _travelDbContext.Offers
            .Include(o => o.AccommodationOffer)
            .Include(o => o.TransportationOffer)
            .Include(o => o.IdMatchNavigation)
            .Include(o => o.UserIdUserNavigation)
            .Where(o => o.Type == type)
            .ToList();
    }

    public IEnumerable<Offer> GetOffersByTypeAndMatch(string type, int idMatch)
    {
        return _travelDbContext.Offers
            .Include(o => o.AccommodationOffer)
            .Include(o => o.TransportationOffer)
            .Include(o => o.IdMatchNavigation)
            .Include(o => o.UserIdUserNavigation)
            .Where(o => o.Type == type && o.IdMatch == idMatch)
            .ToList();
    }

    public Offer? GetChosenOfferByTypeAndMatch(string type, int idMatch)
    {
        return _travelDbContext.Offers
            .Include(o => o.AccommodationOffer)
            .Include(o => o.TransportationOffer)
            .Include(o => o.IdMatchNavigation)
            .Include(o => o.UserIdUserNavigation)
            .Where(o => o.Type == type && o.IdMatch == idMatch && o.Chosen == true)
            .FirstOrDefault();
    }

    public Offer? GetOfferById(int idOffer)
    {
        return _travelDbContext.Offers
            .Include(o => o.AccommodationOffer)
            .Include(o => o.TransportationOffer)
            .Include(o => o.IdMatchNavigation)
            .Include(o => o.UserIdUserNavigation)
            .Where(o => o.IdOffer == idOffer)
            .FirstOrDefault();
    }

    public Offer CreateOffer(Offer offer)
    {
        _travelDbContext.Offers.Add(offer);
        _travelDbContext.SaveChanges();
        return offer;
    }

    public void UpdateOffer(Offer offer)
    {
        _travelDbContext.Offers.Update(offer);
        _travelDbContext.SaveChanges();
    }

    public void DeleteOffer(int idOffer)
    {
        var offer = _travelDbContext.Offers.Find(idOffer);
        if (offer != null)
        {
            _travelDbContext.Offers.Remove(offer);
            _travelDbContext.SaveChanges();
        }
    }
}
