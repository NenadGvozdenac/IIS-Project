namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetSeasonPlayerAverages
{
    public class GetSeasonPlayerAveragesResponse
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? TeamId { get; set; }
        public List<PlayerSeasonAverage> PlayerAverages { get; set; } = new();
        public string Description { get; set; } = "";
    }

    public class PlayerSeasonAverage
    {
        public string PlayerId { get; set; } = "";
        public int MatchesPlayed { get; set; }
        public double AvgPoints { get; set; }
        public double AvgAssists { get; set; }
        public double AvgFouls { get; set; }
        public double PointsStdDev { get; set; }
        public double TotalPoints { get; set; }
        public double TotalAssists { get; set; }
        public double TotalFouls { get; set; }
    }
}