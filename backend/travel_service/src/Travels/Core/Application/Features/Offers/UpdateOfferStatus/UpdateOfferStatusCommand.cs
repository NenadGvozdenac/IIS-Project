using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Offers.UpdateOfferStatus;

public class UpdateOfferStatusCommand : IRequest<Result<UpdateOfferStatusResponse>>
{
    public int IdOffer { get; set; }
    public int IdAgency { get; set; }
    public int IdRequest { get; set; }
    public bool Chosen { get; set; }
    public string Type { get; set; } = string.Empty;
    public int IdMatch { get; set; }
}
