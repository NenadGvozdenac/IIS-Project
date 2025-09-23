using travel_service.src.Travels.Core.Domain.Entities;
using travel_service.src.Travels.Core.Application.Features.Trip.GetTravelCostReport;

namespace travel_service.src.Travels.Core.Application.Interfaces;

public interface ITripRepository
{
    Trip CreateTrip(Trip trip);
    bool DoesMatchExist(int matchId);
    bool DoesTripExistForMatch(int matchId);
    Trip? GetTripByMatch(int matchId);
    bool DoesTransportationOfferExist(int idOffer, int idAgency, int idRequest);
    bool DoesAccommodationOfferExist(int idOffer, int idAgency, int idRequest);
    bool IsTransportationOfferChosen(int idOffer, int idAgency, int idRequest);
    bool IsAccommodationOfferChosen(int idOffer, int idAgency, int idRequest);
    
}
