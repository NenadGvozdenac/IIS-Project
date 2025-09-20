using travel_service.src.Travels.Core.Application.Features.Offers.GetAllOffers;

namespace travel_service.src.Travels.Core.Application.Features.Offers.GetOfferByIds;

public class GetOfferByIdsResponse
{
    public OfferDto? ChosenOffer { get; set; }
    public string Type { get; set; } = string.Empty;
    public int IdMatch { get; set; }
    public bool HasChosenOffer { get; set; }
    public string Message { get; set; } = string.Empty;
}
