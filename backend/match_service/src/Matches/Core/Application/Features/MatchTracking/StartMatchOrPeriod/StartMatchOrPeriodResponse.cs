namespace match_service.src.Matches.Core.Application.Features.MatchTracking.StartMatchOrPeriod;

public class StartMatchOrPeriodResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int MatchId { get; set; }
    public string NewTrackingStatus { get; set; } = string.Empty;
    public string NewPeriodStatus { get; set; } = string.Empty;
    public string CurrentPeriod { get; set; } = string.Empty;
    public DateTime? StartTime { get; set; }
    public DateTime? PeriodStartTime { get; set; }
    public DateTime? LastUpdateTime { get; set; }
}