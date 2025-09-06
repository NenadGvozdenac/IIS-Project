namespace match_service.src.Matches.Core.Application.Features.TeamMemberMatch.UpdateStartingLineup;

public class UpdateStartingLineupResponse
{
    public int MatchId { get; set; }
    public int UpdatedPlayersCount { get; set; }
    public string Message { get; set; } = string.Empty;

    public UpdateStartingLineupResponse(int matchId, int updatedPlayersCount)
    {
        MatchId = matchId;
        UpdatedPlayersCount = updatedPlayersCount;
        Message = $"Successfully updated starting lineup for {updatedPlayersCount} players in match {matchId}";
    }
}
