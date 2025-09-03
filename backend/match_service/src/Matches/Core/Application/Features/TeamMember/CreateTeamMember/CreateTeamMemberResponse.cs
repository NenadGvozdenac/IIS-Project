namespace match_service.src.Matches.Core.Application.Features.TeamMember.CreateTeamMember;

public class CreateTeamMemberResponse
{
    public TeamMemberResponse TeamMember { get; set; }
}

public class TeamMemberResponse
{
    public int IdPlayer { get; set; }
    public int IdTeam { get; set; }
    public int JerseyNumber { get; set; }
    public string? Status { get; set; }
    public string? PlayerName { get; set; }
    public string? PlayerSurname { get; set; }
    public string? TeamName { get; set; }
}
