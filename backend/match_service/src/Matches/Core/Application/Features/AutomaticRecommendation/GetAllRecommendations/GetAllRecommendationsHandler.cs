using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;

namespace match_service.src.Matches.Core.Application.Features.AutomaticRecommendation.GetAllRecommendations
{
    public class GetAllRecommendationsHandler : IRequestHandler<GetAllRecommendationsQuery, Result<GetAllRecommendationsResponse>>
    {
        private readonly IAutomaticRecommendationRepository _automaticRecommendationRepository;

        public GetAllRecommendationsHandler(IAutomaticRecommendationRepository automaticRecommendationRepository)
        {
            _automaticRecommendationRepository = automaticRecommendationRepository;
        }

        public Task<Result<GetAllRecommendationsResponse>> Handle(GetAllRecommendationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Get ALL recommendations for the match (no status filter)
                var recommendations = _automaticRecommendationRepository.GetByMatchId(request.MatchId).ToList();

                var recommendationDtos = recommendations.Select(r => new RecommendationDto
                {
                    IdRecommendation = r.IdRecommendation,
                    Priority = r.Priority ?? string.Empty,
                    Status = r.Status ?? string.Empty,
                    CreationTime = r.CreationTime,
                    Period = r.Period ?? string.Empty,
                    PeriodTime = r.PeriodTime ?? 0,
                    Type = r.Type ?? string.Empty,
                    Description = r.Description ?? string.Empty,
                    IdMatch = r.IdMatch
                }).ToList();

                var response = new GetAllRecommendationsResponse
                {
                    Recommendations = recommendationDtos
                };

                return Task.FromResult(Result<GetAllRecommendationsResponse>.Success(response));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<GetAllRecommendationsResponse>.Failure($"Error getting all recommendations: {ex.Message}"));
            }
        }
    }
}
