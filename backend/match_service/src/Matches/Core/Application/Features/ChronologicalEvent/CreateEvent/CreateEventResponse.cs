namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.CreateEvent
{
    public class CreateEventResponse
    {
        public int EventId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Period { get; set; }
        public int? PeriodTime { get; set; }
        public DateTime CreationTime { get; set; }
        public string? Notes { get; set; }
        public int IdMatch { get; set; }
        public int? IdTeam { get; set; }
        public int? IdPlayer { get; set; }
    }
}