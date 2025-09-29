using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.TeamMember.GetAllTeamMembers
{
    public class GetAllTeamMembersQuery : IRequest<Result<GetAllTeamMembersResponse>>
    {
    }
}
