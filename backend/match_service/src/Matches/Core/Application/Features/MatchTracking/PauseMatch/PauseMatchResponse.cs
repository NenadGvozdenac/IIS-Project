namespace match_service.src.Matches.Core.Application.Features.MatchTracking.PauseMatch;

public class PauseMatchResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int MatchId { get; set; }
    public string NewPeriodStatus { get; set; } = string.Empty;
    public int ElapsedPeriodTime { get; set; }
    public DateTime? LastPauseStartTime { get; set; }
    public DateTime? LastUpdateTime { get; set; }
}