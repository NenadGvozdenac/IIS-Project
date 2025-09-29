using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.TeamMember.GetTeamMemberById
{
    public class GetTeamMemberByIdQuery : IRequest<Result<GetTeamMemberByIdResponse>>
    {
        public int IdPlayer { get; set; }
        public int IdTeam { get; set; }

        public GetTeamMemberByIdQuery(int idPlayer, int idTeam)
        {
            IdPlayer = idPlayer;
            IdTeam = idTeam;
        }
    }
}
