namespace travel_service.src.Travels.Core.Application.Features.Trip.CreateTrip;

public class CreateTripResponse
{
    public int IdTrip { get; set; }
    public string? Notes { get; set; }
    public int MatchIdMatch { get; set; }
    public int IdTransportationOffer { get; set; }
    public int IdTransportationAgency { get; set; }
    public int IdTransportationRequest { get; set; }
    public int? IdAccommodationOffer { get; set; }
    public int? IdAccommodationAgency { get; set; }
    public int? IdAccommodationRequest { get; set; }
}
