namespace match_service.src.Matches.Core.Application.Features.Teams.GetAllTeams;

public class GetAllTeamsResponse
{
    public IEnumerable<TeamDto> Teams { get; set; } = new List<TeamDto>();
}

public class TeamDto
{
    public int IdTeam { get; set; }
    public string Name { get; set; } = null!;
    public string State { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Hall { get; set; } = null!;
    public DateOnly? FoundedDate { get; set; }
    public string? Coach { get; set; }
    public string? KeyStrenghts { get; set; }
    public string? KeyWeaknesses { get; set; }
}
