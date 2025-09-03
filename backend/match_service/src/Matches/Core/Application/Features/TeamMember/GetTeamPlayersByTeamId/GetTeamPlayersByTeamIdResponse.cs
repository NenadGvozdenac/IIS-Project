namespace match_service.src.Matches.Core.Application.Features.TeamMember.GetTeamPlayersByTeamId;

public class GetTeamPlayersByTeamIdResponse
{
    public List<TeamPlayerDto> TeamPlayers { get; set; } = new List<TeamPlayerDto>();
}

public class TeamPlayerDto
{
    public int PlayerId { get; set; }
    public string? PlayerName { get; set; }
    public string? PlayerSurname { get; set; }
    public DateOnly? Birthday { get; set; }
    public int? Weight { get; set; }
    public int? Height { get; set; }
    public string? PositionName { get; set; }
    public string? NationalityName { get; set; }
    public int? JerseyNumber { get; set; }
    public string? Status { get; set; }
    public int? Age 
    { 
        get 
        {
            if (Birthday.HasValue)
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                var age = today.Year - Birthday.Value.Year;
                if (Birthday.Value > today.AddYears(-age)) age--;
                return age;
            }
            return null;
        }
    }
}
