using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;

namespace match_service.src.Matches.Core.Application.Features.AutomaticRecommendation.AcceptRecommendation
{
    public class AcceptRecommendationHandler : IRequestHandler<AcceptRecommendationCommand, Result<AcceptRecommendationResponse>>
    {
        private readonly IAutomaticRecommendationRepository _automaticRecommendationRepository;

        public AcceptRecommendationHandler(IAutomaticRecommendationRepository automaticRecommendationRepository)
        {
            _automaticRecommendationRepository = automaticRecommendationRepository;
        }

        public Task<Result<AcceptRecommendationResponse>> Handle(AcceptRecommendationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var recommendation = _automaticRecommendationRepository.GetById(request.RecommendationId);
                if (recommendation == null)
                {
                    return Task.FromResult(Result<AcceptRecommendationResponse>.Failure("Recommendation not found"));
                }

                if (recommendation.Status != "pending")
                {
                    return Task.FromResult(Result<AcceptRecommendationResponse>.Failure("Only pending recommendations can be accepted"));
                }

                recommendation.Status = "accepted";
                var updatedRecommendation = _automaticRecommendationRepository.Update(recommendation);

                var response = new AcceptRecommendationResponse
                {
                    IdRecommendation = updatedRecommendation.IdRecommendation,
                    Status = updatedRecommendation.Status ?? string.Empty,
                    Message = "Recommendation accepted successfully"
                };

                return Task.FromResult(Result<AcceptRecommendationResponse>.Success(response));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<AcceptRecommendationResponse>.Failure($"Error accepting recommendation: {ex.Message}"));
            }
        }
    }
}