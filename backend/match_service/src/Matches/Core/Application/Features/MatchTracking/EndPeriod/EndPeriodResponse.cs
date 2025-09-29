namespace match_service.src.Matches.Core.Application.Features.MatchTracking.EndPeriod;

public class EndPeriodResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int MatchId { get; set; }
    public string EndedPeriod { get; set; } = string.Empty;
    public string NewPeriodStatus { get; set; } = string.Empty;
    public bool IsLastPeriod { get; set; }
    public DateTime? LastUpdateTime { get; set; }
}