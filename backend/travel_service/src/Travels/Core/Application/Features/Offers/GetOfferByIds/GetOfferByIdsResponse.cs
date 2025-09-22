namespace travel_service.src.Travels.Core.Application.Features.Offers.GetOfferByIds;

public class GetOfferByIdsResponse
{
    public OfferDto? ChosenOffer { get; set; }
    public string Type { get; set; } = string.Empty;
    public int IdMatch { get; set; }
    public bool HasChosenOffer { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class OfferDto
{
    public int IdOffer { get; set; }
    public int? Price { get; set; }
    public int? UserIdUser { get; set; }
    public int IdMatch { get; set; }
    public int IdAgency { get; set; }
    public int IdRequest { get; set; }
    public string? Type { get; set; }
    public bool? Chosen { get; set; }
    
    // Agency information
    public string? AgencyName { get; set; }
    
    // Accommodation specific properties
    public string? Name { get; set; }
    public int? Capacity { get; set; }
    public string? AccommodationType { get; set; }
    public bool? DoubleRoom { get; set; }
    public bool? TripleRoom { get; set; }
    public bool? QuadrupleRoom { get; set; }
    public bool? Breakfast { get; set; }
    public bool? FitnessCenter { get; set; }
    public bool? Pool { get; set; }
    public bool? Wifi { get; set; }
    public bool? Spa { get; set; }
    
    // Transportation specific properties
    public string? CompanyName { get; set; }
    public string? VehicleType { get; set; }
    public bool? EquipmentSpace { get; set; }
    public bool? AirConditioning { get; set; }
    public bool? Tv { get; set; }
    public bool? WifiTransport { get; set; }
    public bool? Restroom { get; set; }
}