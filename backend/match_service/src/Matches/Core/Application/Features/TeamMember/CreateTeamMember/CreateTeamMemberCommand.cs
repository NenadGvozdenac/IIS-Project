using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.TeamMember.CreateTeamMember
{
    public class CreateTeamMemberCommand : IRequest<Result<CreateTeamMemberResponse>>
    {
        public int? JerseyNumber { get; set; }
        public string? Status { get; set; }
        public int IdPlayer { get; set; }
        public int IdTeam { get; set; }
    }
}
