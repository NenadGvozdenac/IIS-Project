using MediatR;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Infrastructure;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.MatchTracking.StartMatchOrPeriod;

public class StartMatchOrPeriodHandler : IRequestHandler<StartMatchOrPeriodCommand, Result<StartMatchOrPeriodResponse>>
{
    private readonly IMatchTrackingRepository _matchTrackingRepository;
    private readonly MatchDbContext _context;

    public StartMatchOrPeriodHandler(
        IMatchTrackingRepository matchTrackingRepository,
        MatchDbContext context)
    {
        _matchTrackingRepository = matchTrackingRepository;
        _context = context;
    }

    public Task<Result<StartMatchOrPeriodResponse>> Handle(StartMatchOrPeriodCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var matchTracking = _matchTrackingRepository.GetByMatchId(request.MatchId);
            if (matchTracking == null)
            {
                return Task.FromResult(Result<StartMatchOrPeriodResponse>.Failure("Match tracking not found"));
            }

            var now = DateTime.UtcNow;
            var isMatchStart = matchTracking.StartTime == null;
            
            // Set StartTime only if not already set (first time starting)
            if (isMatchStart)
            {
                matchTracking.StartTime = now;
                matchTracking.CurrentPeriod = "1"; // Start with first period
            }

            // Set match as active
            matchTracking.TrackingStatus = "active";
            matchTracking.PeriodStatus = "active";
            matchTracking.PeriodStartTime = now;
            matchTracking.LastUpdateTime = now;

            // Reset period-specific timers
            matchTracking.ElapsedPeriodTime = 0;
            matchTracking.TotalPauseTimeInPeriod = 0;
            matchTracking.LastPauseStartTime = null;

            _matchTrackingRepository.Update(matchTracking);
            _context.SaveChanges();

            var response = new StartMatchOrPeriodResponse
            {
                Success = true,
                Message = isMatchStart ? "Match started successfully" : "Period started successfully",
                MatchId = request.MatchId,
                NewTrackingStatus = matchTracking.TrackingStatus,
                NewPeriodStatus = matchTracking.PeriodStatus,
                CurrentPeriod = matchTracking.CurrentPeriod ?? "1",
                StartTime = matchTracking.StartTime,
                PeriodStartTime = matchTracking.PeriodStartTime,
                LastUpdateTime = matchTracking.LastUpdateTime
            };

            return Task.FromResult(Result<StartMatchOrPeriodResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<StartMatchOrPeriodResponse>.Failure($"Error starting match/period: {ex.Message}"));
        }
    }
}