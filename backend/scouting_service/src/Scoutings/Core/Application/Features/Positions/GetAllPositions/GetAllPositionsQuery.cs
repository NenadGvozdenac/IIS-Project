using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.Positions.GetAllPositions;

public class GetAllPositionsQuery : IRequest<Result<List<GetAllPositionsResponse>>>
{
}
