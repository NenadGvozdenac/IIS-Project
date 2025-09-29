using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.TeamMember.UpdateTeamMember
{
    public class UpdateTeamMemberCommand : IRequest<Result<UpdateTeamMemberResponse>>
    {
        public int? JerseyNumber { get; set; }
        public string? Status { get; set; }
        public int IdPlayer { get; set; }
        public int IdTeam { get; set; }
    }
}
