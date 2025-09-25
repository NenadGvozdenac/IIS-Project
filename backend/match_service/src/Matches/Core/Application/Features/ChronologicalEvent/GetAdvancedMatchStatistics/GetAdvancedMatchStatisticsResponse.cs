namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetAdvancedMatchStatistics
{
    public class GetAdvancedMatchStatisticsResponse
    {
        public string MatchId { get; set; } = string.Empty;
        public List<PeriodStatistics> PeriodStatistics { get; set; } = new List<PeriodStatistics>();
        public string Description { get; set; } = "Period-based player efficiency analysis with filtering, grouping, aggregation and sorting";
    }

    public class PeriodStatistics
    {
        public string Period { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string TeamId { get; set; } = string.Empty;
        public int Count { get; set; }
        //public DateTime? Timestamp { get; set; }
        public double EfficiencyRating { get; set; }
    }
}