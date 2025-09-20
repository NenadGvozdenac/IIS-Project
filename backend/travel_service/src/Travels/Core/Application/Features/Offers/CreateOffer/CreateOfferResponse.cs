namespace travel_service.src.Travels.Core.Application.Features.Offers.CreateOffer;

public class CreateOfferResponse
{
    public int IdOffer { get; set; }
    public string Type { get; set; } = string.Empty;
    public int? Price { get; set; }
    public int IdMatch { get; set; }
    public int IdAgency { get; set; }
    public int IdRequest { get; set; }
    public int UserId { get; set; }
    public string Message { get; set; } = "Offer created successfully";
}
