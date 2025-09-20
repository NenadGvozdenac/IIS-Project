namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetTeamStatistics
{
    public class GetTeamStatisticsResponse
    {
        public string PlayingStyle { get; set; } = string.Empty;
        public string HeadToHead { get; set; } = string.Empty;
        public TeamPerformanceStats Performance { get; set; } = new();
        public List<string> KeyStrengths { get; set; } = new();
        public List<string> KeyWeaknesses { get; set; } = new();
    }

    public class TeamPerformanceStats
    {
        public ShotStats TwoPoint { get; set; } = new();
        public ShotStats ThreePoint { get; set; } = new();
        public ShotStats FreeThrow { get; set; } = new();
        public ReboundStats Rebounds { get; set; } = new();
        public GeneralStats General { get; set; } = new();
    }

    public class ShotStats
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

    public class GeneralStats
    {
        public double AssistsAvg { get; set; }
        public double TurnoversAvg { get; set; }
        public double StealsAvg { get; set; }
        public double BlocksAvg { get; set; }
        public double PointsAvg { get; set; }
        public int TotalGames { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public double WinPercentage { get; set; }
    }
}