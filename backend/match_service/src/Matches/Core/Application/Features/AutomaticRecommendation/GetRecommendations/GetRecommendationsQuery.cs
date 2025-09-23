using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.AutomaticRecommendation.GetRecommendations
{
    public class GetRecommendationsQuery : IRequest<Result<GetRecommendationsResponse>>
    {
        public int MatchId { get; set; }

        public GetRecommendationsQuery(int matchId)
        {
            MatchId = matchId;
        }
    }
}