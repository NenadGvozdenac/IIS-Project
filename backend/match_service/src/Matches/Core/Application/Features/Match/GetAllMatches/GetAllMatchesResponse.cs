namespace match_service.src.Matches.Core.Application.Features.Match.GetAllMatches;

public class GetAllMatchesResponse
{
    public List<MatchDto> Matches { get; set; } = new List<MatchDto>();
}

public class MatchDto
{
    public int IdMatch { get; set; }
    public string Name { get; set; } = null!;
    public DateTime ScheduledAt { get; set; }
    public string Type { get; set; } = null!;
    public string State { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Hall { get; set; } = null!;
    public bool IsInOurHall { get; set; }
    public int IdTeam { get; set; }
    public string? TeamName { get; set; }
    public string? CompetitionName { get; set; }
    public string? SeasonName { get; set; }
    public string Place => IsInOurHall ? "Home" : "Opponent";
    public string FormattedDate => ScheduledAt.ToString("dd.MM.yyyy, HH:mm");
    
    // MatchTracking data
    public string? TrackingStatus { get; set; }
    public int? OurPoints { get; set; }
    public int? OpponentPoints { get; set; }
    public string? CurrentPeriod { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}
