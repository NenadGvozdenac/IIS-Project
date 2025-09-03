namespace travel_service.src.Travels.Core.Application.Features.Matches.GetAllMatches;

public class GetAllMatchesResponse
{
    public List<MatchDto> Matches { get; set; } = new List<MatchDto>();
}

public class MatchDto
{
    public int IdMatch { get; set; }
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; }
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
