using MediatR;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Infrastructure;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.MatchTracking.NextPeriod;

public class NextPeriodHandler : IRequestHandler<NextPeriodCommand, Result<NextPeriodResponse>>
{
    private readonly IMatchTrackingRepository _matchTrackingRepository;
    private readonly MatchDbContext _context;

    public NextPeriodHandler(
        IMatchTrackingRepository matchTrackingRepository,
        MatchDbContext context)
    {
        _matchTrackingRepository = matchTrackingRepository;
        _context = context;
    }

    public Task<Result<NextPeriodResponse>> Handle(NextPeriodCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var matchTracking = _matchTrackingRepository.GetByMatchId(request.MatchId);
            if (matchTracking == null)
            {
                return Task.FromResult(Result<NextPeriodResponse>.Failure("Match tracking not found"));
            }

            var previousPeriod = matchTracking.CurrentPeriod ?? "1";
            var nextPeriod = GetNextPeriod(previousPeriod);

            if (nextPeriod == null)
            {
                return Task.FromResult(Result<NextPeriodResponse>.Failure("Cannot advance to next period - already at final period"));
            }

            var now = DateTime.UtcNow;

            // Update to next period
            matchTracking.CurrentPeriod = nextPeriod;
            matchTracking.PeriodStartTime = now;
            matchTracking.LastUpdateTime = now;
            
            // Reset period-specific timers
            matchTracking.ElapsedPeriodTime = 0;
            matchTracking.TotalPauseTimeInPeriod = 0;
            matchTracking.LastPauseStartTime = null;
            
            // Set period as upcoming (waiting to start)
            matchTracking.PeriodStatus = "upcoming";

            _matchTrackingRepository.Update(matchTracking);
            _context.SaveChanges();

            var response = new NextPeriodResponse
            {
                Success = true,
                Message = $"Advanced to {GetPeriodDisplayName(nextPeriod)}",
                MatchId = request.MatchId,
                PreviousPeriod = GetPeriodDisplayName(previousPeriod),
                CurrentPeriod = GetPeriodDisplayName(nextPeriod),
                PeriodStartTime = matchTracking.PeriodStartTime,
                LastUpdateTime = matchTracking.LastUpdateTime
            };

            return Task.FromResult(Result<NextPeriodResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<NextPeriodResponse>.Failure($"Error advancing to next period: {ex.Message}"));
        }
    }

    private string? GetNextPeriod(string currentPeriod)
    {
        return currentPeriod switch
        {
            "1" => "2",
            "2" => "3", 
            "3" => "4",
            "4" => "end",
            _ => null // Already at end or invalid period
        };
    }

    private string GetPeriodDisplayName(string period)
    {
        return period switch
        {
            "1" => "1st Quarter",
            "2" => "2nd Quarter",
            "3" => "3rd Quarter", 
            "4" => "4th Quarter",
            "end" => "Game End",
            _ => period
        };
    }
}