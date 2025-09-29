using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.MatchTracking.GetMatchEvents
{
    public class GetMatchEventsCommand : IRequest<Result<GetMatchEventsResponse>>
    {
        public int MatchId { get; set; }
    }
}