using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetMatchEventsInflux
{
    public class GetMatchEventsInfluxResponse
    {
        public List<ChronologicalEventInflux> Events { get; set; } = new List<ChronologicalEventInflux>();
        public int TotalCount { get; set; }
        public string MatchId { get; set; } = string.Empty;
        public Dictionary<string, int> EventTypeCounts { get; set; } = new Dictionary<string, int>();
    }
}