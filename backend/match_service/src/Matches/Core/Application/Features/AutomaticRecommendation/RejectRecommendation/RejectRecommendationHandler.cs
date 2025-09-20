using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;

namespace match_service.src.Matches.Core.Application.Features.AutomaticRecommendation.RejectRecommendation
{
    public class RejectRecommendationHandler : IRequestHandler<RejectRecommendationCommand, Result<RejectRecommendationResponse>>
    {
        private readonly IAutomaticRecommendationRepository _automaticRecommendationRepository;

        public RejectRecommendationHandler(IAutomaticRecommendationRepository automaticRecommendationRepository)
        {
            _automaticRecommendationRepository = automaticRecommendationRepository;
        }

        public Task<Result<RejectRecommendationResponse>> Handle(RejectRecommendationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var recommendation = _automaticRecommendationRepository.GetById(request.RecommendationId);
                if (recommendation == null)
                {
                    return Task.FromResult(Result<RejectRecommendationResponse>.Failure("Recommendation not found"));
                }

                if (recommendation.Status != "pending")
                {
                    return Task.FromResult(Result<RejectRecommendationResponse>.Failure("Only pending recommendations can be rejected"));
                }

                recommendation.Status = "rejected";
                var updatedRecommendation = _automaticRecommendationRepository.Update(recommendation);

                var response = new RejectRecommendationResponse
                {
                    IdRecommendation = updatedRecommendation.IdRecommendation,
                    Status = updatedRecommendation.Status ?? string.Empty,
                    Message = "Recommendation rejected successfully"
                };

                return Task.FromResult(Result<RejectRecommendationResponse>.Success(response));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<RejectRecommendationResponse>.Failure($"Error rejecting recommendation: {ex.Message}"));
            }
        }
    }
}