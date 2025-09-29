namespace travel_service.src.Travels.Core.Application.Features.Matches.GetMatchById;

public class GetMatchByIdResponse
{
    public int IdMatch { get; set; }
    public string? Name { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string? Type { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? Hall { get; set; }
    public bool IsInOurHall { get; set; }
    public bool TransportationRequired { get; set; }
    public bool AccommodationRequired { get; set; }
    public int? IdCompetition { get; set; }
    public int IdSeason { get; set; }
    public int IdTeam { get; set; }
}
