namespace match_service.src.Matches.Core.Application.Features.MatchTracking.NextPeriod;

public class NextPeriodResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int MatchId { get; set; }
    public string PreviousPeriod { get; set; } = string.Empty;
    public string CurrentPeriod { get; set; } = string.Empty;
    public DateTime? PeriodStartTime { get; set; }
    public DateTime? LastUpdateTime { get; set; }
}