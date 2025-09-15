using MediatR;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Infrastructure;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.MatchTracking.EndPeriod;

public class EndPeriodHandler : IRequestHandler<EndPeriodCommand, Result<EndPeriodResponse>>
{
    private readonly IMatchTrackingRepository _matchTrackingRepository;
    private readonly MatchDbContext _context;

    public EndPeriodHandler(
        IMatchTrackingRepository matchTrackingRepository,
        MatchDbContext context)
    {
        _matchTrackingRepository = matchTrackingRepository;
        _context = context;
    }

    public Task<Result<EndPeriodResponse>> Handle(EndPeriodCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var matchTracking = _matchTrackingRepository.GetByMatchId(request.MatchId);
            if (matchTracking == null)
            {
                return Task.FromResult(Result<EndPeriodResponse>.Failure("Match tracking not found"));
            }

            var currentPeriod = matchTracking.CurrentPeriod ?? "1";
            var isLastPeriod = currentPeriod == "4";

            var now = DateTime.UtcNow;

            // Set period status as finished
            matchTracking.PeriodStatus = "finished";
            
            // Preserve elapsed period time and update last update time
            matchTracking.LastUpdateTime = now;
            
            // Clear pause time since period is ending
            matchTracking.LastPauseStartTime = null;

            _matchTrackingRepository.Update(matchTracking);
            _context.SaveChanges();

            var response = new EndPeriodResponse
            {
                Success = true,
                Message = isLastPeriod ? "Final period ended" : $"{GetPeriodDisplayName(currentPeriod)} ended",
                MatchId = request.MatchId,
                EndedPeriod = GetPeriodDisplayName(currentPeriod),
                NewPeriodStatus = matchTracking.PeriodStatus,
                IsLastPeriod = isLastPeriod,
                LastUpdateTime = matchTracking.LastUpdateTime
            };

            return Task.FromResult(Result<EndPeriodResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<EndPeriodResponse>.Failure($"Error ending period: {ex.Message}"));
        }
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