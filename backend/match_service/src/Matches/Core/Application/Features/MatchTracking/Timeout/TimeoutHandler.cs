using MediatR;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Infrastructure;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Application.Features.MatchTracking.Timeout;

public class TimeoutHandler : IRequestHandler<TimeoutCommand, Result<TimeoutResponse>>
{
    private readonly IMatchTrackingRepository _matchTrackingRepository;
    private readonly ITeamEventRepository _teamEventRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly MatchDbContext _context;

    public TimeoutHandler(
        IMatchTrackingRepository matchTrackingRepository,
        ITeamEventRepository teamEventRepository,
        ITeamRepository teamRepository,
        MatchDbContext context)
    {
        _matchTrackingRepository = matchTrackingRepository;
        _teamEventRepository = teamEventRepository;
        _teamRepository = teamRepository;
        _context = context;
    }

    public async Task<Result<TimeoutResponse>> Handle(TimeoutCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var matchTracking = _matchTrackingRepository.GetByMatchId(request.MatchId);
            if (matchTracking == null)
            {
                return Result<TimeoutResponse>.Failure("Match tracking not found");
            }

            // Can only call timeout if period is currently active
            if (matchTracking.PeriodStatus != "active")
            {
                return Result<TimeoutResponse>.Failure($"Cannot call timeout. Current period status: {matchTracking.PeriodStatus}");
            }

            if (!matchTracking.PeriodStartTime.HasValue)
            {
                return Result<TimeoutResponse>.Failure("Cannot call timeout - period start time not set");
            }

            // Get team information
            var team = _teamRepository.GetById(request.TeamId);
            if (team == null)
            {
                return Result<TimeoutResponse>.Failure("Team not found");
            }

            var now = DateTime.UtcNow;

            // Calculate elapsed time in current period (excluding previous pauses)
            var periodElapsed = (int)(now - matchTracking.PeriodStartTime.Value).TotalSeconds;
            var totalPreviousPauses = matchTracking.TotalPauseTimeInPeriod ?? 0;
            var netElapsedTime = Math.Max(0, periodElapsed - totalPreviousPauses);

            // Update tracking state - set to paused due to timeout
            matchTracking.PeriodStatus = "paused";
            matchTracking.ElapsedPeriodTime = netElapsedTime;
            matchTracking.LastPauseStartTime = now;
            matchTracking.LastUpdateTime = now;

            // Create team event for timeout
            var teamEvent = new TeamEvent
            {
                CreationTime = now,
                Notes = $"Timeout called by {team.Name}",
                Type = "timeout",
                Period = matchTracking.CurrentPeriod,
                PeriodTime = netElapsedTime,
                IdTeam = request.TeamId,
                IdMatch = request.MatchId
            };

            // Add event to context and update match tracking
            _teamEventRepository.Create(teamEvent);
            _matchTrackingRepository.Update(matchTracking);

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var response = new TimeoutResponse
            {
                Success = true,
                Message = "Timeout called successfully",
                MatchId = request.MatchId,
                TeamId = request.TeamId,
                TeamName = team.Name ?? string.Empty,
                TimeoutTime = now,
                ElapsedPeriodTime = netElapsedTime,
                CurrentPeriod = matchTracking.CurrentPeriod ?? string.Empty
            };

            return Result<TimeoutResponse>.Success(response);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<TimeoutResponse>.Failure($"Error calling timeout: {ex.Message}");
        }
    }
}