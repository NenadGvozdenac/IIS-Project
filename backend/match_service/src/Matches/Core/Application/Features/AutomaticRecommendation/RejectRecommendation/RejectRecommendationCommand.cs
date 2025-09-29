using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.AutomaticRecommendation.RejectRecommendation
{
    public class RejectRecommendationCommand : IRequest<Result<RejectRecommendationResponse>>
    {
        public int RecommendationId { get; set; }

        public RejectRecommendationCommand(int recommendationId)
        {
            RecommendationId = recommendationId;
        }
    }
}