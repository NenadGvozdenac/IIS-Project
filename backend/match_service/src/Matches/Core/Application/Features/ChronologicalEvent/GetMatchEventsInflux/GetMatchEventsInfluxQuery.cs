using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetMatchEventsInflux
{
    public class GetMatchEventsInfluxQuery : IRequest<Result<GetMatchEventsInfluxResponse>>
    {
        public string MatchId { get; set; } = string.Empty;
        public string? EventCategory { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Period { get; set; }
    }
}