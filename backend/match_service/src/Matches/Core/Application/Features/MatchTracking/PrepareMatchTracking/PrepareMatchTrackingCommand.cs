using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.MatchTracking.PrepareMatchTracking
{
    public class PrepareMatchTrackingCommand : IRequest<Result<PrepareMatchTrackingResponse>>
    {
        public int MatchId { get; set; }
        public List<int> OurTeamPlayerIds { get; set; } = new();
        public List<int> OpponentTeamPlayerIds { get; set; } = new();
    }
}
