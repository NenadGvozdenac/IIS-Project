using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Offers.GetAllOffers;

public record GetAllOffersQuery(string Type, int IdMatch) : IRequest<Result<GetAllOffersResponse>>;
