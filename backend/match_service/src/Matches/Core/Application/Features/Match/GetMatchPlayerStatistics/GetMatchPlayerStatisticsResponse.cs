namespace match_service.src.Matches.Core.Application.Features.Match.GetMatchPlayerStatistics;

public class GetMatchPlayerStatisticsResponse
{
    public List<PlayerEfficiencyDto> PlayerStatistics { get; set; } = new List<PlayerEfficiencyDto>();
    public int MatchId { get; set; }
    public string Message { get; set; } = string.Empty;
}
public class PlayerEfficiencyDto
{
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    public int MatchId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string MatchName { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public int TotalAssists { get; set; }
    public int TotalRebounds { get; set; }
    public int TotalSteals { get; set; }
    public int TotalBlocks { get; set; }
    public int TotalFouls { get; set; }
    public int Shooting2PMade { get; set; }
    public int Shooting2PAttempted { get; set; }
    public decimal Shooting2PPercentage { get; set; }
    public int Shooting3PMade { get; set; }
    public int Shooting3PAttempted { get; set; }
    public decimal Shooting3PPercentage { get; set; }
    public int FreeThrowsMade { get; set; }
    public int FreeThrowsAttempted { get; set; }
    public decimal FreeThrowPercentage { get; set; }
    public int OffensiveRebounds { get; set; }
    public int DefensiveRebounds { get; set; }
    public int SubstitutionsCount { get; set; }
    public decimal EfficiencyRating { get; set; }
    public string PerformanceGrade { get; set; } = string.Empty;
    public int MinutesPlayed { get; set; }
    public decimal PlusMinusRating { get; set; }
}