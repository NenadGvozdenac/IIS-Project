using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Commands.AutoSelectBestOffer;

public class AutoSelectBestOfferCommand : IRequest<Result<AutoSelectBestOfferResponse>>
{
    public int MatchId { get; set; }
    public string OfferType { get; set; } = string.Empty; // "transportation" or "accommodation"
    public decimal WeightPrice { get; set; } = 0.4m;
    public decimal WeightCapacity { get; set; } = 0.3m;
    public decimal WeightBenefits { get; set; } = 0.2m;
    public decimal WeightAgency { get; set; } = 0.1m;
}

public class AutoSelectBestOfferResponse
{
    public int? SelectedOfferId { get; set; }
    public string SelectedAgencyName { get; set; } = string.Empty;
    public decimal SelectionScore { get; set; }
    public string SelectionReason { get; set; } = string.Empty;
    public int TotalOffersAnalyzed { get; set; }
}