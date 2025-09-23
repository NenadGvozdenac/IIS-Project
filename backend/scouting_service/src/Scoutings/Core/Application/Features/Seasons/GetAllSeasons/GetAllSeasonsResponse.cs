namespace scouting_service.src.Scoutings.Core.Application.Features.Seasons.GetAllSeasons;

public class GetAllSeasonsResponse
{
    public List<SeasonDto> Seasons { get; set; } = new();
}

public class SeasonDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly StartedAt { get; set; }
    public DateOnly? EndedAt { get; set; }
}
