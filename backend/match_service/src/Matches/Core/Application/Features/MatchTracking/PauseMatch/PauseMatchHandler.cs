using MediatR;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Infrastructure;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.MatchTracking.PauseMatch;

public class PauseMatchHandler : IRequestHandler<PauseMatchCommand, Result<PauseMatchResponse>>
{
    private readonly IMatchTrackingRepository _matchTrackingRepository;
    private readonly MatchDbContext _context;

    public PauseMatchHandler(
        IMatchTrackingRepository matchTrackingRepository,
        MatchDbContext context)
    {
        _matchTrackingRepository = matchTrackingRepository;
        _context = context;
    }

    public Task<Result<PauseMatchResponse>> Handle(PauseMatchCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var matchTracking = _matchTrackingRepository.GetByMatchId(request.MatchId);
            if (matchTracking == null)
            {
                return Task.FromResult(Result<PauseMatchResponse>.Failure("Match tracking not found"));
            }

            // Can only pause if period is currently active
            if (matchTracking.PeriodStatus != "active")
            {
                return Task.FromResult(Result<PauseMatchResponse>.Failure($"Cannot pause match. Current period status: {matchTracking.PeriodStatus}"));
            }

            if (!matchTracking.PeriodStartTime.HasValue)
            {
                return Task.FromResult(Result<PauseMatchResponse>.Failure("Cannot pause - period start time not set"));
            }

            var now = DateTime.UtcNow;

            // Calculate elapsed time in current period (excluding previous pauses)
            var periodElapsed = (int)(now - matchTracking.PeriodStartTime.Value).TotalSeconds;
            var totalPreviousPauses = matchTracking.TotalPauseTimeInPeriod ?? 0;
            var netElapsedTime = Math.Max(0, periodElapsed - totalPreviousPauses);

            // Update tracking state
            matchTracking.PeriodStatus = "paused";
            matchTracking.ElapsedPeriodTime = netElapsedTime;
            matchTracking.LastPauseStartTime = now;
            matchTracking.LastUpdateTime = now;

            _matchTrackingRepository.Update(matchTracking);
            _context.SaveChanges();

            var response = new PauseMatchResponse
            {
                Success = true,
                Message = "Match paused successfully",
                MatchId = request.MatchId,
                NewPeriodStatus = matchTracking.PeriodStatus,
                ElapsedPeriodTime = netElapsedTime,
                LastPauseStartTime = matchTracking.LastPauseStartTime,
                LastUpdateTime = matchTracking.LastUpdateTime
            };

            return Task.FromResult(Result<PauseMatchResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<PauseMatchResponse>.Failure($"Error pausing match: {ex.Message}"));
        }
    }
}