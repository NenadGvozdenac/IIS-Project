namespace match_service.src.Matches.Core.Application.Features.TeamMemberMatch.GetTeamMembersByMatch;

public class GetTeamMembersByMatchResponse
{
    public List<TeamMemberDto> TeamMembers { get; set; } = new();
}

public class TeamMemberDto
{
    public int IdMatch { get; set; }
    public int IdTeam { get; set; }
    public int IdPlayer { get; set; }
    public string PlayerName { get; set; } = null!;
    public string PlayerSurname { get; set; } = null!;
    public int? JerseyNumber { get; set; }
    public string? PositionName { get; set; }
    public bool InGame { get; set; }
    public bool StartingLineup { get; set; }
    public int? Age { get; set; }
    public string TeamName { get; set; } = null!;
}
