namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Competitions.CreateCompetition;

public class CreateCompetitionResponse
{
    public int IdCompetition { get; set; }
    public string Name { get; set; } = null!;
    public DateOnly StartedAt { get; set; }
    public DateOnly? EndedAt { get; set; }
    public int NumberOfMatches { get; set; }
    public string Message { get; set; } = null!;
}
