namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetPlayerPerformanceComparison
{
    public class GetPlayerPerformanceComparisonResponse
    {
        public string MatchId { get; set; } = string.Empty;
        public List<PlayerPerformance> PlayerRankings { get; set; } = new List<PlayerPerformance>();
        public string Description { get; set; } = "Player performance comparison with ranking by total points and aggregation functions";
    }

    public class PlayerPerformance
    {
        public string PlayerId { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public int TotalEvents { get; set; }
        public int Rank { get; set; }
        public double TotalPoints { get; set; }
        public double PerformanceScore { get; set; }
        public Dictionary<string, int> EventTypeBreakdown { get; set; } = new Dictionary<string, int>();
    }
}