namespace travel_service.src.Travels.Core.Application.Features.Seasons.GetSeasonById;

public class GetSeasonByIdResponse
{
    public int IdSeason { get; set; }
    public DateOnly StartedAt { get; set; }
    public DateOnly? EndedAt { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
}
