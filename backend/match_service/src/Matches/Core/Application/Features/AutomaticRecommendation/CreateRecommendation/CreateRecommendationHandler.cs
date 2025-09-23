using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Application.Utilities;
using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Application.Features.AutomaticRecommendation.CreateRecommendation
{
    public class CreateRecommendationHandler : IRequestHandler<CreateRecommendationCommand, Result<CreateRecommendationResponse>>
    {
        private readonly IAutomaticRecommendationRepository _automaticRecommendationRepository;
        private readonly IMatchTrackingRepository _matchTrackingRepository;

        public CreateRecommendationHandler(
            IAutomaticRecommendationRepository automaticRecommendationRepository,
            IMatchTrackingRepository matchTrackingRepository)
        {
            _automaticRecommendationRepository = automaticRecommendationRepository;
            _matchTrackingRepository = matchTrackingRepository;
        }

        public Task<Result<CreateRecommendationResponse>> Handle(CreateRecommendationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get match tracking for period and time information
                var matchTracking = _matchTrackingRepository.GetByMatchId(request.MatchId);
                if (matchTracking == null)
                {
                    return Task.FromResult(Result<CreateRecommendationResponse>.Failure("Match tracking not found"));
                }

                // Calculate remaining time in current period for event tracking
                var periodTime = PeriodTimeCalculator.CalculateRemainingPeriodTime(matchTracking);

                var automaticRecommendation = new Domain.Entities.AutomaticRecommendation
                {
                    Priority = request.Priority,
                    Status = "pending",
                    CreationTime = DateTime.UtcNow,
                    Period = matchTracking.CurrentPeriod,
                    PeriodTime = periodTime,
                    Type = request.Type,
                    Description = request.Description,
                    IdMatch = request.MatchId
                };

                var createdRecommendation = _automaticRecommendationRepository.Create(automaticRecommendation);

                var response = new CreateRecommendationResponse
                {
                    IdRecommendation = createdRecommendation.IdRecommendation,
                    Priority = createdRecommendation.Priority ?? string.Empty,
                    Status = createdRecommendation.Status ?? string.Empty,
                    CreationTime = createdRecommendation.CreationTime,
                    Period = createdRecommendation.Period ?? string.Empty,
                    PeriodTime = createdRecommendation.PeriodTime ?? 0,
                    Type = createdRecommendation.Type ?? string.Empty,
                    Description = createdRecommendation.Description ?? string.Empty,
                    IdMatch = createdRecommendation.IdMatch
                };

                return Task.FromResult(Result<CreateRecommendationResponse>.Success(response));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<CreateRecommendationResponse>.Failure($"Error creating recommendation: {ex.Message}"));
            }
        }
    }
}