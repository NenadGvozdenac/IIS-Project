using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.UndoLastEvent
{
    public class UndoLastEventCommand : IRequest<Result<UndoLastEventResponse>>
    {
        public int MatchId { get; set; }
        public int TeamId { get; set; }
    }
}