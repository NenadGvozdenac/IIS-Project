namespace match_service.src.Matches.Core.Application.Features.Match.GetCompleteMatchReport;

public class GetCompleteMatchReportResponse
{
    public MatchGeneralInfoDto GeneralInfo { get; set; } = new MatchGeneralInfoDto();
    public TeamMatchStatsDto OurTeamStats { get; set; } = new TeamMatchStatsDto();
    public TeamMatchStatsDto OpponentTeamStats { get; set; } = new TeamMatchStatsDto();
    public List<PlayerEfficiencyDto> OurPlayersStats { get; set; } = new List<PlayerEfficiencyDto>();
    public List<PlayerEfficiencyDto> OpponentPlayersStats { get; set; } = new List<PlayerEfficiencyDto>();
    public int TotalPlayersCount { get; set; }
    public string MatchSummary { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class MatchGeneralInfoDto
{
    public int MatchId { get; set; }
    public string MatchName { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public string Hall { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public int TotalEventsCount { get; set; }
    public int HighestIndividualScore { get; set; }
    public int LowestIndividualScore { get; set; }
    public int TotalSubstitutions { get; set; }
    public int TotalFouls { get; set; }
    public int OurTeamId { get; set; }
    public int OpponentTeamId { get; set; }
    public int FinalScoreOur { get; set; }
    public int FinalScoreOpponent { get; set; }
}

public class TeamMatchStatsDto
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public int TotalFieldGoalsMade { get; set; }
    public int TotalFieldGoalsAttempted { get; set; }
    public decimal FieldGoalPercentage { get; set; }
    public int Total2PMade { get; set; }
    public int Total2PAttempted { get; set; }
    public decimal TwoPointPercentage { get; set; }
    public int Total3PMade { get; set; }
    public int Total3PAttempted { get; set; }
    public decimal ThreePointPercentage { get; set; }
    public int TotalFreeThrowsMade { get; set; }
    public int TotalFreeThrowsAttempted { get; set; }
    public decimal FreeThrowPercentage { get; set; }
    public int TotalRebounds { get; set; }
    public int TotalOffensiveRebounds { get; set; }
    public int TotalDefensiveRebounds { get; set; }
    public int TotalAssists { get; set; }
    public int TotalSteals { get; set; }
    public int TotalBlocks { get; set; }
    public int TotalFouls { get; set; }
    public decimal TeamEfficiencyRating { get; set; }
    public int ActivePlayersCount { get; set; }
    public int SubstitutionsCount { get; set; }
    public decimal AvgPlayerEfficiency { get; set; }
    public string BestPlayerName { get; set; } = string.Empty;
    public decimal BestPlayerEfficiency { get; set; }
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