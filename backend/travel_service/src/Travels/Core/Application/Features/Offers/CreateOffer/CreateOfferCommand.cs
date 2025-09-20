using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Offers.CreateOffer;

public class CreateOfferCommand : IRequest<Result<CreateOfferResponse>>
{
    public int UserId { get; set; }
    public int? Price { get; set; }
    public int IdMatch { get; set; }
    public int IdAgency { get; set; }
    public int IdRequest { get; set; }
    public string Type { get; set; } = string.Empty;
    
    // Accommodation Offer properties
    public string? Name { get; set; }
    public int? Capacity { get; set; }
    public string? AccommodationType { get; set; }
    public bool DoubleRoom { get; set; }
    public bool TripleRoom { get; set; }
    public bool QuadrupleRoom { get; set; }
    public bool Breakfast { get; set; }
    public bool FitnessCenter { get; set; }
    public bool Pool { get; set; }
    public bool Wifi { get; set; }
    public bool Spa { get; set; }
    
    // Transportation Offer properties
    public string? CompanyName { get; set; }
    public string? VehicleType { get; set; }
    public bool EquipmentSpace { get; set; }
    public bool AirConditioning { get; set; }
    public bool Tv { get; set; }
    public bool WifiTransport { get; set; }
    public bool Restroom { get; set; }
}
