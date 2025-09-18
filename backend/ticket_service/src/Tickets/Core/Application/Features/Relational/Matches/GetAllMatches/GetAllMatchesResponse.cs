namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Matches.GetAllMatches;

public class GetAllMatchesResponse
{
    public int IdMatch { get; set; }
    public string Name { get; set; } = null!;
    public DateTime ScheduledAt { get; set; }
    public string Type { get; set; } = null!;
    public string State { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Hall { get; set; } = null!;
    public bool IsInOurHall { get; set; }
    public bool TransportationRequired { get; set; }
    public bool AccommodationRequired { get; set; }
    
    // Related entities
    public string? CompetitionName { get; set; }
    public string SeasonName { get; set; } = null!;
    public string TeamName { get; set; } = null!;
}
