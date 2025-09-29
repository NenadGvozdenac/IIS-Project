namespace match_service.src.Matches.Core.Application.Features.TeamMemberMatch.PlayerSubstitution;

public class PlayerSubstitutionResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int MatchId { get; set; }
    public int TeamId { get; set; }
    public string PlayerInName { get; set; } = string.Empty;
    public string PlayerOutName { get; set; } = string.Empty;
    public DateTime SubstitutionTime { get; set; }
}