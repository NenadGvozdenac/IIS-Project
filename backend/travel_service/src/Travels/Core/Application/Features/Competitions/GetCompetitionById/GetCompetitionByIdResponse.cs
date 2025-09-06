namespace travel_service.src.Travels.Core.Application.Features.Competitions.GetCompetitionById;

public class GetCompetitionByIdResponse
{
    public int IdCompetition { get; set; }
    public string Name { get; set; } = null!;
    public DateOnly StartedAt { get; set; }
    public DateOnly? EndedAt { get; set; }
}
