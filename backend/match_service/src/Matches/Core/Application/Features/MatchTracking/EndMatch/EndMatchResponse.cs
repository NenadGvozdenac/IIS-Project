namespace match_service.src.Matches.Core.Application.Features.MatchTracking.EndMatch;

public class EndMatchResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int MatchId { get; set; }
    public string NewTrackingStatus { get; set; } = string.Empty;
    public string NewPeriodStatus { get; set; } = string.Empty;
    public string FinalPeriod { get; set; } = string.Empty;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime? LastUpdateTime { get; set; }
    public int? OurPoints { get; set; }
    public int? OpponentPoints { get; set; }
}