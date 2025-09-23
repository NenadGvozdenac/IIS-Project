using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.AutomaticRecommendation.AcceptRecommendation
{
    public class AcceptRecommendationCommand : IRequest<Result<AcceptRecommendationResponse>>
    {
        public int RecommendationId { get; set; }

        public AcceptRecommendationCommand(int recommendationId)
        {
            RecommendationId = recommendationId;
        }
    }
}