using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;
using travel_service.src.Travels.Core.Infrastructure;
using travel_service.src.Travels.Core.Application.Features.Trip.GetTravelCostReport;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text;
using System;

namespace travel_service.src.Travels.Core.Infrastructure.Repositories;

public class TripRepository : ITripRepository
{
    private readonly TravelDbContext _travelDbContext;

    public TripRepository(TravelDbContext travelDbContext)
    {
        _travelDbContext = travelDbContext;
    }

    public Trip CreateTrip(Trip trip)
    {
        _travelDbContext.Trips.Add(trip);
        _travelDbContext.SaveChanges();
        return trip;
    }

    public bool DoesMatchExist(int matchId)
    {
        return _travelDbContext.Matches.Any(m => m.IdMatch == matchId);
    }

    public bool DoesTripExistForMatch(int matchId)
    {
        return _travelDbContext.Trips.Any(t => t.MatchIdMatch == matchId);
    }

    public Trip? GetTripByMatch(int matchId)
    {
        return _travelDbContext.Trips.FirstOrDefault(t => t.MatchIdMatch == matchId);
    }

    public bool DoesTransportationOfferExist(int idOffer, int idAgency, int idRequest)
    {
        return _travelDbContext.Offers.Any(o => 
            o.IdOffer == idOffer && 
            o.IdAgency == idAgency && 
            o.IdRequest == idRequest && 
            o.Type == "transportation");
    }

    public bool DoesAccommodationOfferExist(int idOffer, int idAgency, int idRequest)
    {
        return _travelDbContext.Offers.Any(o => 
            o.IdOffer == idOffer && 
            o.IdAgency == idAgency && 
            o.IdRequest == idRequest && 
            o.Type == "accommodation");
    }

    public bool IsTransportationOfferChosen(int idOffer, int idAgency, int idRequest)
    {
        return _travelDbContext.Offers.Any(o => 
            o.IdOffer == idOffer && 
            o.IdAgency == idAgency && 
            o.IdRequest == idRequest && 
            o.Type == "transportation" && 
            o.Chosen == true);
    }

    public bool IsAccommodationOfferChosen(int idOffer, int idAgency, int idRequest)
    {
        return _travelDbContext.Offers.Any(o => 
            o.IdOffer == idOffer && 
            o.IdAgency == idAgency && 
            o.IdRequest == idRequest && 
            o.Type == "accommodation" && 
            o.Chosen == true);
    }

}
