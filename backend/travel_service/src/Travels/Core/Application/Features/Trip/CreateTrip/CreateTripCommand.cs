using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Trip.CreateTrip;

public class CreateTripCommand : IRequest<Result<CreateTripResponse>>
{
    public string? Notes { get; set; }
    public int MatchIdMatch { get; set; }
    
    // Obavezni prevoz
    public int IdTransportationOffer { get; set; }
    public int IdTransportationAgency { get; set; }
    public int IdTransportationRequest { get; set; }
    
    // Opcioni smeštaj
    public int? IdAccommodationOffer { get; set; }
    public int? IdAccommodationAgency { get; set; }
    public int? IdAccommodationRequest { get; set; }
}
