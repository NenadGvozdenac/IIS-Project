using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.AutomaticRecommendation.CreateRecommendation
{
    public class CreateRecommendationCommand : IRequest<Result<CreateRecommendationResponse>>
    {
        public int MatchId { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}