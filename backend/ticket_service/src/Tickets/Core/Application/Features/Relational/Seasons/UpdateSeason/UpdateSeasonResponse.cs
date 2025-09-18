namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Seasons.UpdateSeason;

public class UpdateSeasonResponse
{
    public int IdSeason { get; set; }
    public DateOnly StartedAt { get; set; }
    public DateOnly? EndedAt { get; set; }
    public string Name { get; set; } = null!;
    public string Message { get; set; } = null!;
}
