using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.MatchTracking.GetMatchTrackingByMatchId;

public class GetMatchTrackingByMatchIdQuery : IRequest<Result<GetMatchTrackingByMatchIdResponse>>
{
    public int MatchId { get; set; }

    public GetMatchTrackingByMatchIdQuery(int matchId)
    {
        MatchId = matchId;
    }
}
