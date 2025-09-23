namespace match_service.src.Matches.Core.Application.Features.MatchTracking.Timeout;

public class TimeoutResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int MatchId { get; set; }
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public DateTime TimeoutTime { get; set; }
    public int ElapsedPeriodTime { get; set; }
    public string CurrentPeriod { get; set; } = string.Empty;
}