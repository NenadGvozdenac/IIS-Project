namespace match_service.src.Matches.Core.Application.Features.MatchTracking.GetMatchTrackingByMatchId;

public class GetMatchTrackingByMatchIdResponse
{
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? TrackingStatus { get; set; }
    public int? PeriodDuration { get; set; }
    public string? CurrentPeriod { get; set; }
    public string? PeriodStatus { get; set; }
    public DateTime? PeriodStartTime { get; set; }
    public int? ElapsedPeriodTime { get; set; }
    public int? RemainingPeriodTime { get; set; }
    public DateTime? LastPauseStartTime { get; set; }
    public int? TotalPauseTimeInPeriod { get; set; }
    public DateTime? LastUpdateTime { get; set; }
    public int? OurPoints { get; set; }
    public int? OpponentPoints { get; set; }
    public int? IdUser { get; set; }
    public int IdMatch { get; set; }
    public string? MatchName { get; set; }
    public string? UserName { get; set; }
}
