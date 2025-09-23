using System.Collections.Generic;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetPlayerStatistics;

public class GetPlayerStatisticsResponse
{
    public int PlayerId { get; set; }
    public string? PlayerName { get; set; }
    public string? Position { get; set; }
    public int JerseyNumber { get; set; }
    public PlayerPerformance? Performance { get; set; }
    public string? PlayingStyle { get; set; }
}

public class PlayerPerformance
{
    public ShotPerformance? TwoPoint { get; set; }
    public ShotPerformance? ThreePoint { get; set; }
    public ShotPerformance? FreeThrow { get; set; }
    public ReboundStats? Rebounds { get; set; }
    public PlayerGeneralStats? General { get; set; }
}

public class ShotPerformance
{
    public int Made { get; set; }
    public int Attempted { get; set; }
    public double Percentage { get; set; }
}

public class ReboundStats
{
    public int Offensive { get; set; }
    public int Defensive { get; set; }
    public int Total { get; set; }
}

public class PlayerGeneralStats
{
    public int TotalGames { get; set; }
    public double PointsAvg { get; set; }
    public double AssistsAvg { get; set; }
    public double StealsAvg { get; set; }
    public double BlocksAvg { get; set; }
    public double TurnoversAvg { get; set; }
    public double FoulsAvg { get; set; }
}