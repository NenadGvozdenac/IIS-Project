using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.TeamMemberMatch.UpdateStartingLineup;

public class UpdateStartingLineupCommand : IRequest<Result<UpdateStartingLineupResponse>>
{
    public int MatchId { get; set; }
    public List<StartingLineupPlayerRequest> Players { get; set; } = new();

    public UpdateStartingLineupCommand(int matchId, List<StartingLineupPlayerRequest> players)
    {
        MatchId = matchId;
        Players = players;
    }
}

public class StartingLineupPlayerRequest
{
    public int TeamId { get; set; }
    public int PlayerId { get; set; }
    public bool StartingLineup { get; set; }
    public bool InGame { get; set; }
}
