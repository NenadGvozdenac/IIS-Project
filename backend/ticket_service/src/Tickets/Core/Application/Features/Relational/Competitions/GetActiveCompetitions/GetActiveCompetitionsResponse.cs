namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Competitions.GetActiveCompetitions;

public class GetActiveCompetitionsResponse
{
    public int IdCompetition { get; set; }
    public string Name { get; set; } = null!;
    public DateOnly StartedAt { get; set; }
    public DateOnly? EndedAt { get; set; }
    public int NumberOfMatches { get; set; }
    public int CurrentMatches { get; set; }
}
