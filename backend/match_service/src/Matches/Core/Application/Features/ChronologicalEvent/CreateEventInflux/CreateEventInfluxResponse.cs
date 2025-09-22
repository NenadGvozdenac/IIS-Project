namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.CreateEventInflux
{
    public class CreateEventInfluxResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string MatchId { get; set; } = string.Empty;
        public int EventId { get; set; }
    }
}