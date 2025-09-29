namespace match_service.src.Matches.Core.Application.Features.Match.GetMatchById;

public class GetMatchByIdResponse
{
    public int IdMatch { get; set; }
    public string Name { get; set; } = null!;
    public DateTime ScheduledAt { get; set; }
    public string Type { get; set; } = null!;
    public string State { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Hall { get; set; } = null!;
    public bool IsInOurHall { get; set; }
    public int? IdCompetition { get; set; }
    public int IdSeason { get; set; }
    public int IdTeam { get; set; }
    public string? TeamName { get; set; }
    public string? CompetitionName { get; set; }
    public string? SeasonName { get; set; }
    
    // MatchTracking data
    public string? TrackingStatus { get; set; }
    public int? OurPoints { get; set; }
    public int? OpponentPoints { get; set; }
    public string? CurrentPeriod { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    // Additional MatchTracking details
    public int? PeriodDuration { get; set; }
    public string? PeriodStatus { get; set; }
    public DateTime? PeriodStartTime { get; set; }
    public int? ElapsedPeriodTime { get; set; }
    public DateTime? LastPauseStartTime { get; set; }
    public int? TotalPauseTimeInPeriod { get; set; }
    public DateTime? LastUpdateTime { get; set; }
    public int? TrackingUserId { get; set; }
}
