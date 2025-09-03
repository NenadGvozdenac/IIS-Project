namespace match_service.src.Matches.Core.Application.Features.TeamMember.GetTeamMemberById;

public class GetTeamMemberByIdResponse
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
    public DateOnly? PlayerBirthday { get; set; }
    public int? PlayerWeight { get; set; }
    public int? PlayerHeight { get; set; }
    public string? PlayerNationality { get; set; }
    public string? PlayerPosition { get; set; }
    public string? TeamName { get; set; }
}
