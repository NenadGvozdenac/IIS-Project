using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Features.Players.GetAllPlayers;

namespace travel_service.src.Travels.Core.Application.Features.TeamMembers.GetAllTeamMembers;
public class GetAllTeamMembersQuery : IRequest<Result<List<GetAllTeamMembersResponse>>>
{
}