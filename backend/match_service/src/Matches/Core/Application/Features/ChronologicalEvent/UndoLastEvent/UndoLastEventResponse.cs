namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.UndoLastEvent
{
    public class UndoLastEventResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? UndoneEventId { get; set; }
        public string? UndoneEventType { get; set; }
        public int? PointsRolledBack { get; set; }
        public int MatchId { get; set; }
        public int TeamId { get; set; }
    }
}