using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Players.GetAllPlayers;
public class GetAllPlayersQuery : IRequest<Result<List<GetAllPlayersResponse>>>
{
}