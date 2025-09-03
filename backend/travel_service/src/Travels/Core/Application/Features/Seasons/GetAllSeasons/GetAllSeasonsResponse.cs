namespace travel_service.src.Travels.Core.Application.Features.Seasons.GetAllSeasons;

public class GetAllSeasonsResponse
{
    public int IdSeason { get; set; }
    public DateOnly StartedAt { get; set; }
    public DateOnly? EndedAt { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
}
