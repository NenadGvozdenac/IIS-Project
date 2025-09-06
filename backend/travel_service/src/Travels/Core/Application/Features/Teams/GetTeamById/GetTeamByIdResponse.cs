namespace travel_service.src.Travels.Core.Application.Features.Teams.GetTeamById;

public class GetTeamByIdResponse
{
    public int IdTeam { get; set; }
    public string Name { get; set; } = null!;
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string Hall { get; set; } = null!;
}
