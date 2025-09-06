namespace travel_service.src.Travels.Core.Application.Features.TeamMembers.GetAllTeamMembers;

public class GetAllTeamMembersResponse
{
    public int? JerseyNumber { get; set; }

    public string? Status { get; set; }

    public int IdPlayer { get; set; }

    public int IdTeam { get; set; }

    public string FullName { get; set; } = null!;   
}