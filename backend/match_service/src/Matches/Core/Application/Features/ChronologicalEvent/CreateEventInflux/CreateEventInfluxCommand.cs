using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.CreateEventInflux
{
    public class CreateEventInfluxCommand : IRequest<Result<CreateEventInfluxResponse>>
    {
        public string MatchId { get; set; } = string.Empty;
        public string EventCategory { get; set; } = string.Empty; // "personal", "team", "general"
        public string EventType { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public int? PeriodTime { get; set; }
        public string? TeamId { get; set; }
        public string? PlayerId { get; set; }
        public string? PlayerName { get; set; }
        public int EventId { get; set; }
        public string? Notes { get; set; }
        public int? OurPoints { get; set; }
        public int? OpponentPoints { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}