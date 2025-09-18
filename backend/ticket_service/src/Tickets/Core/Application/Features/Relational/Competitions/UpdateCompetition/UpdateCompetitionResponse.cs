namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Competitions.UpdateCompetition;

public class UpdateCompetitionResponse
{
    public int IdCompetition { get; set; }
    public string Name { get; set; } = null!;
    public DateOnly StartedAt { get; set; }
    public DateOnly? EndedAt { get; set; }
    public int NumberOfMatches { get; set; }
    public string Message { get; set; } = null!;
}
