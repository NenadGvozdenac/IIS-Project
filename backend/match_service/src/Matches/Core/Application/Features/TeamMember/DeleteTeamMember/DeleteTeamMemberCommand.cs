using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.TeamMember.DeleteTeamMember
{
    public class DeleteTeamMemberCommand : IRequest<Result<DeleteTeamMemberResponse>>
    {
        public int IdPlayer { get; set; }
        public int IdTeam { get; set; }

        public DeleteTeamMemberCommand(int idPlayer, int idTeam)
        {
            IdPlayer = idPlayer;
            IdTeam = idTeam;
        }
    }
}
