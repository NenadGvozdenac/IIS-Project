namespace match_service.src.Matches.Core.Application.Features.Teams.GetTeamById;

public class GetTeamByIdResponse
{
    public int IdTeam { get; set; }
    public string? Name { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? Hall { get; set; }
    public DateOnly? FoundedDate { get; set; }
    public string? Coach { get; set; }
    public string? KeyStrenghts { get; set; }
    public string? KeyWeaknesses { get; set; }
}
