namespace match_service.src.Matches.Core.Application.Features.Match.StartMatch;

public class StartMatchResponse
{
    public int MatchId { get; set; }
    public string TrackingStatus { get; set; } = null!;
    public int CreatedTeamMemberMatches { get; set; }
    public string Message { get; set; } = null!;
}
