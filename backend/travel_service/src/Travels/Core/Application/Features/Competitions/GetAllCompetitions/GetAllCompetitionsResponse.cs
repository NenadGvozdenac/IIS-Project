namespace travel_service.src.Travels.Core.Application.Features.Competitions.GetAllCompetitions;

public class GetAllCompetitionsResponse
{
    public int IdCompetition { get; set; }
    public string Name { get; set; } = null!;
    public DateOnly StartedAt { get; set; }
    public DateOnly? EndedAt { get; set; }
    public int NumberOfMatches { get; set; }
    public bool IsActive { get; set; }
    public List<MatchInfo> Matches { get; set; } = new();
}

public class MatchInfo
{
    public int IdMatch { get; set; }
    public string Name { get; set; } = null!;
    public DateTime? Date { get; set; }
}
