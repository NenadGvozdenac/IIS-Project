using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.Position.GetAllPositions;

public record GetAllPositionsQuery() : IRequest<Result<GetAllPositionsResponse>>;
