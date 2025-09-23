using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.Players.GetAllPlayers;

public class GetAllPlayersQuery : IRequest<Result<List<GetAllPlayersResponse>>>
{
}
