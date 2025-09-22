using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Offers.GetOfferByIds;

public record GetOfferByIdsQuery(string Type, int IdMatch) : IRequest<Result<GetOfferByIdsResponse>>;
