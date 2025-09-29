using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Application.Utilities;

namespace match_service.src.Matches.Core.Application.Features.MatchTracking.GetMatchTrackingByMatchId;

public class GetMatchTrackingByMatchIdHandler : IRequestHandler<GetMatchTrackingByMatchIdQuery, Result<GetMatchTrackingByMatchIdResponse>>
{
    private readonly IMatchTrackingRepository _matchTrackingRepository;

    public GetMatchTrackingByMatchIdHandler(IMatchTrackingRepository matchTrackingRepository)
    {
        _matchTrackingRepository = matchTrackingRepository;
    }

    public Task<Result<GetMatchTrackingByMatchIdResponse>> Handle(GetMatchTrackingByMatchIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var matchTracking = _matchTrackingRepository.GetByMatchId(request.MatchId);
            
            if (matchTracking == null)
            {
                return Task.FromResult(Result<GetMatchTrackingByMatchIdResponse>.Failure("Match tracking not found"));
            }

            var response = new GetMatchTrackingByMatchIdResponse
            {
                StartTime = matchTracking.StartTime,
                EndTime = matchTracking.EndTime,
                TrackingStatus = matchTracking.TrackingStatus,
                PeriodDuration = matchTracking.PeriodDuration,
                CurrentPeriod = matchTracking.CurrentPeriod,
                PeriodStatus = matchTracking.PeriodStatus,
                PeriodStartTime = matchTracking.PeriodStartTime,
                ElapsedPeriodTime = matchTracking.ElapsedPeriodTime,
                RemainingPeriodTime = PeriodTimeCalculator.CalculateRemainingPeriodTime(matchTracking),
                LastPauseStartTime = matchTracking.LastPauseStartTime,
                TotalPauseTimeInPeriod = matchTracking.TotalPauseTimeInPeriod,
                LastUpdateTime = matchTracking.LastUpdateTime,
                OurPoints = matchTracking.OurPoints,
                OpponentPoints = matchTracking.OpponentPoints,
                IdUser = matchTracking.IdUser,
                IdMatch = matchTracking.IdMatch,
                MatchName = matchTracking.IdMatchNavigation?.Name,
                UserName = matchTracking.IdUserNavigation?.Name
            };

            return Task.FromResult(Result<GetMatchTrackingByMatchIdResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetMatchTrackingByMatchIdResponse>.Failure($"Failed to get match tracking: {ex.Message}"));
        }
    }
}
