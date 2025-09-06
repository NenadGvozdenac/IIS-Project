using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.TeamMemberMatch.GetTeamMembersByMatch;

public class GetTeamMembersByMatchQuery : IRequest<Result<GetTeamMembersByMatchResponse>>
{
    public int MatchId { get; set; }
    public int? TeamId { get; set; } // Optional - if null, returns all teams

    public GetTeamMembersByMatchQuery(int matchId, int? teamId = null)
    {
        MatchId = matchId;
        TeamId = teamId;
    }
}
