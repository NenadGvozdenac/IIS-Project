using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.TravelInformations.GetTravelInfoById;

public record GetTravelInfoByIdQuery(int Id) : IRequest<Result<GetTravelInfoByIdResponse>>;
