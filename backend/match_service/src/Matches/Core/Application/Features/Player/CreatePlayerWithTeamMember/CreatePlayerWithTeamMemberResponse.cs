namespace match_service.src.Matches.Core.Application.Features.Player.CreatePlayerWithTeamMember;

public class CreatePlayerWithTeamMemberResponse
{
    public int PlayerId { get; set; }
    public string? PlayerName { get; set; }
    public string? PlayerSurname { get; set; }
    public int? JerseyNumber { get; set; }
    public string? Status { get; set; }
}
