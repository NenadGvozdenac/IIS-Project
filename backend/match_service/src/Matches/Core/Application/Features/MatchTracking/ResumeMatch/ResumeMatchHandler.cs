using MediatR;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Infrastructure;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.MatchTracking.ResumeMatch;

public class ResumeMatchHandler : IRequestHandler<ResumeMatchCommand, Result<ResumeMatchResponse>>
{
    private readonly IMatchTrackingRepository _matchTrackingRepository;
    private readonly MatchDbContext _context;

    public ResumeMatchHandler(
        IMatchTrackingRepository matchTrackingRepository,
        MatchDbContext context)
    {
        _matchTrackingRepository = matchTrackingRepository;
        _context = context;
    }

    public Task<Result<ResumeMatchResponse>> Handle(ResumeMatchCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var matchTracking = _matchTrackingRepository.GetByMatchId(request.MatchId);
            if (matchTracking == null)
            {
                return Task.FromResult(Result<ResumeMatchResponse>.Failure("Match tracking not found"));
            }

            // Can only resume if period is currently paused
            if (matchTracking.PeriodStatus != "paused")
            {
                return Task.FromResult(Result<ResumeMatchResponse>.Failure($"Cannot resume match. Current period status: {matchTracking.PeriodStatus}"));
            }

            if (!matchTracking.LastPauseStartTime.HasValue)
            {
                return Task.FromResult(Result<ResumeMatchResponse>.Failure("Cannot resume - last pause start time not set"));
            }

            var now = DateTime.UtcNow;

            // Calculate duration of this pause in milliseconds
            var pauseDuration = (int)(now - matchTracking.LastPauseStartTime.Value).TotalMilliseconds;
            
            // Add to total pause time for this period
            var newTotalPauseTime = (matchTracking.TotalPauseTimeInPeriod ?? 0) + pauseDuration;

            // Update tracking state
            matchTracking.PeriodStatus = "active";
            matchTracking.TotalPauseTimeInPeriod = newTotalPauseTime;
            matchTracking.LastUpdateTime = now;
            
            // Clear the pause start time since we're resuming
            matchTracking.LastPauseStartTime = null;

            _matchTrackingRepository.Update(matchTracking);
            _context.SaveChanges();

            var response = new ResumeMatchResponse
            {
                Success = true,
                Message = "Match resumed successfully",
                MatchId = request.MatchId,
                NewPeriodStatus = matchTracking.PeriodStatus,
                TotalPauseTimeInPeriod = newTotalPauseTime,
                PauseDuration = pauseDuration,
                LastUpdateTime = matchTracking.LastUpdateTime
            };

            return Task.FromResult(Result<ResumeMatchResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<ResumeMatchResponse>.Failure($"Error resuming match: {ex.Message}"));
        }
    }
}