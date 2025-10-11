using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.AutomaticRecommendation.GetAllRecommendations
{
    public class GetAllRecommendationsQuery : IRequest<Result<GetAllRecommendationsResponse>>
    {
        public int MatchId { get; set; }

        public GetAllRecommendationsQuery(int matchId)
        {
            MatchId = matchId;
        }
    }
}
