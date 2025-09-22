namespace travel_service.src.Travels.Core.Application.Features.Trip.GetTripByMatch;

public class GetTripByMatchResponse
{
    public bool HasTrip { get; set; }
    public TripDto? Trip { get; set; }
}

public class TripDto
{
    public int IdTrip { get; set; }
    public string? Notes { get; set; }
    public int MatchIdMatch { get; set; }
    public int? IdAccommodationOffer { get; set; }
    public int? IdTransportationOffer { get; set; }
    public int? IdAccommodationAgency { get; set; }
    public int? IdTransportationAgency { get; set; }
    public int? IdAccommodationRequest { get; set; }
    public int? IdTransportationRequest { get; set; }
}