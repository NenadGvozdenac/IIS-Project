using MediatR;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Infrastructure;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.MatchTracking.EndMatch;

public class EndMatchHandler : IRequestHandler<EndMatchCommand, Result<EndMatchResponse>>
{
    private readonly IMatchTrackingRepository _matchTrackingRepository;
    private readonly MatchDbContext _context;

    public EndMatchHandler(
        IMatchTrackingRepository matchTrackingRepository,
        MatchDbContext context)
    {
        _matchTrackingRepository = matchTrackingRepository;
        _context = context;
    }

    public Task<Result<EndMatchResponse>> Handle(EndMatchCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var matchTracking = _matchTrackingRepository.GetByMatchId(request.MatchId);
            if (matchTracking == null)
            {
                return Task.FromResult(Result<EndMatchResponse>.Failure("Match tracking not found"));
            }

            var now = DateTime.UtcNow;

            // Set match as finished
            matchTracking.EndTime = now;
            matchTracking.TrackingStatus = "finished";
            matchTracking.CurrentPeriod = "end";
            matchTracking.PeriodStatus = "finished";
            
            // Reset all timing fields
            matchTracking.ElapsedPeriodTime = 0;
            matchTracking.PeriodStartTime = null;
            matchTracking.LastPauseStartTime = null;
            matchTracking.TotalPauseTimeInPeriod = 0;
            matchTracking.LastUpdateTime = now;

            _matchTrackingRepository.Update(matchTracking);
            _context.SaveChanges();

            var response = new EndMatchResponse
            {
                Success = true,
                Message = "Match ended successfully",
                MatchId = request.MatchId,
                NewTrackingStatus = matchTracking.TrackingStatus,
                NewPeriodStatus = matchTracking.PeriodStatus,
                FinalPeriod = matchTracking.CurrentPeriod,
                StartTime = matchTracking.StartTime,
                EndTime = matchTracking.EndTime,
                LastUpdateTime = matchTracking.LastUpdateTime,
                OurPoints = matchTracking.OurPoints,
                OpponentPoints = matchTracking.OpponentPoints
            };

            return Task.FromResult(Result<EndMatchResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<EndMatchResponse>.Failure($"Error ending match: {ex.Message}"));
        }
    }
}