using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;

namespace match_service.src.Matches.Core.Application.Features.AutomaticRecommendation.GetRecommendations
{
    public class GetRecommendationsHandler : IRequestHandler<GetRecommendationsQuery, Result<GetRecommendationsResponse>>
    {
        private readonly IAutomaticRecommendationRepository _automaticRecommendationRepository;

        public GetRecommendationsHandler(IAutomaticRecommendationRepository automaticRecommendationRepository)
        {
            _automaticRecommendationRepository = automaticRecommendationRepository;
        }

        public Task<Result<GetRecommendationsResponse>> Handle(GetRecommendationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var recommendations = _automaticRecommendationRepository.GetByMatchId(request.MatchId)
                  .Where(r => r.Status == "pending").ToList();

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

                var response = new GetRecommendationsResponse
                {
                    Recommendations = recommendationDtos
                };

                return Task.FromResult(Result<GetRecommendationsResponse>.Success(response));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<GetRecommendationsResponse>.Failure($"Error getting recommendations: {ex.Message}"));
            }
        }
    }
}