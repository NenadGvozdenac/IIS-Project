using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.Match.StartMatch;

public class StartMatchCommand : IRequest<Result<StartMatchResponse>>
{
    public int MatchId { get; set; }
    public List<int> OurTeamPlayerIds { get; set; } = new List<int>();
    public List<int> OpponentTeamPlayerIds { get; set; } = new List<int>();

    public StartMatchCommand(int matchId, List<int> ourTeamPlayerIds, List<int> opponentTeamPlayerIds)
    {
        MatchId = matchId;
        OurTeamPlayerIds = ourTeamPlayerIds;
        OpponentTeamPlayerIds = opponentTeamPlayerIds;
    }
}
