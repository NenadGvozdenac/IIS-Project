namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Competitions.GetCompetitionById;

public class GetCompetitionByIdResponse
{
    public int IdCompetition { get; set; }
    public string Name { get; set; } = null!;
    public DateOnly StartedAt { get; set; }
    public DateOnly? EndedAt { get; set; }
    public int NumberOfMatches { get; set; }
    public bool IsActive { get; set; }
    public List<DetailedMatchInfo> Matches { get; set; } = new();
}

public class DetailedMatchInfo
{
    public int IdMatch { get; set; }
    public string Name { get; set; } = null!;
    public DateTime? Date { get; set; }
    public string? Location { get; set; }
    public string? Status { get; set; }
}
