using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Seasons.GetSeasonById;

public record GetSeasonByIdQuery(int Id) : IRequest<Result<GetSeasonByIdResponse>>;
