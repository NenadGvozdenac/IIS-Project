using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Matches.GetMatchById;

public record GetMatchByIdQuery(int Id) : IRequest<Result<GetMatchByIdResponse>>;
