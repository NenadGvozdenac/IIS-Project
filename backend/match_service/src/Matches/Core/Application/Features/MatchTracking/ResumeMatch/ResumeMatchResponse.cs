namespace match_service.src.Matches.Core.Application.Features.MatchTracking.ResumeMatch;

public class ResumeMatchResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int MatchId { get; set; }
    public string NewPeriodStatus { get; set; } = string.Empty;
    public int TotalPauseTimeInPeriod { get; set; }
    public int PauseDuration { get; set; }
    public DateTime? LastUpdateTime { get; set; }
}