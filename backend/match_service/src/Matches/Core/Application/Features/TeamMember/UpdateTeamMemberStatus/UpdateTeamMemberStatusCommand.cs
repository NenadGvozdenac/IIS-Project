using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.TeamMember.UpdateTeamMemberStatus
{
    public class UpdateTeamMemberStatusCommand : IRequest<Result<UpdateTeamMemberStatusResponse>>
    {
        public int IdPlayer { get; set; }
        public int IdTeam { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
