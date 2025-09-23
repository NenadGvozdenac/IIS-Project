using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.CreateEvent
{
    public class CreateEventCommand : IRequest<Result<CreateEventResponse>>
    {
        public int MatchId { get; set; }
        public string Category { get; set; } = string.Empty; // "personal", "team", "general"
        public string Type { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? Period { get; set; }
        public int? PeriodTime { get; set; }
        
        // For personal events
        public int? PlayerId { get; set; }
        public int? TeamId { get; set; }
    }
}