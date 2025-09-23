namespace match_service.src.Matches.Core.Application.Features.MatchTracking.GetMatchEvents
{
    public class GetMatchEventsResponse
    {
        public List<UnifiedEventDto> Events { get; set; } = new List<UnifiedEventDto>();
    }

    public class UnifiedEventDto
    {
        public int Id { get; set; }
        public string EventType { get; set; } = string.Empty; // "personal", "team", "general"
        public string Type { get; set; } = string.Empty; // substitution, timeout, foul, etc.
        public DateTime CreationTime { get; set; }
        public string? Period { get; set; }
        public int? PeriodTime { get; set; }
        public string? Notes { get; set; }
        
        // Player info (for personal events)
        public int? PlayerId { get; set; }
        public string? PlayerName { get; set; }
        
        // Team info (for team events)
        public int? TeamId { get; set; }
        public string? TeamName { get; set; }
    }
}