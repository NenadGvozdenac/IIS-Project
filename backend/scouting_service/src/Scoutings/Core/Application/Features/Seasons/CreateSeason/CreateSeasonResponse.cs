namespace scouting_service.src.Scoutings.Core.Application.Features.Seasons.CreateSeason;

public class CreateSeasonResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public DateOnly StartedAt { get; set; }
    public DateOnly? EndedAt { get; set; }
}
