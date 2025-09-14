using MediatR;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Infrastructure;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Domain.Entities;
using match_service.src.Matches.Core.Application.Utilities;

namespace match_service.src.Matches.Core.Application.Features.TeamMemberMatch.PlayerSubstitution;

public class PlayerSubstitutionHandler : IRequestHandler<PlayerSubstitutionCommand, Result<PlayerSubstitutionResponse>>
{
    private readonly ITeamMemberMatchRepository _teamMemberMatchRepository;
    private readonly IPersonalEventRepository _personalEventRepository;
    private readonly IMatchTrackingRepository _matchTrackingRepository;
    private readonly MatchDbContext _context;

    public PlayerSubstitutionHandler(
        ITeamMemberMatchRepository teamMemberMatchRepository,
        IPersonalEventRepository personalEventRepository,
        IMatchTrackingRepository matchTrackingRepository,
        MatchDbContext context)
    {
        _teamMemberMatchRepository = teamMemberMatchRepository;
        _personalEventRepository = personalEventRepository;
        _matchTrackingRepository = matchTrackingRepository;
        _context = context;
    }

    public async Task<Result<PlayerSubstitutionResponse>> Handle(PlayerSubstitutionCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            // Get the player coming out (should be in game)
            var playerOut = _teamMemberMatchRepository.GetById(request.MatchId, request.TeamId, request.PlayerOutId);
            if (playerOut == null)
            {
                return Result<PlayerSubstitutionResponse>.Failure("Player going out not found");
            }

            if (!playerOut.InGame)
            {
                return Result<PlayerSubstitutionResponse>.Failure("Player going out is not currently in the game");
            }

            // Get the player coming in (should not be in game)
            var playerIn = _teamMemberMatchRepository.GetById(request.MatchId, request.TeamId, request.PlayerInId);
            if (playerIn == null)
            {
                return Result<PlayerSubstitutionResponse>.Failure("Player coming in not found");
            }

            if (playerIn.InGame)
            {
                return Result<PlayerSubstitutionResponse>.Failure("Player coming in is already in the game");
            }

            // Perform the substitution
            playerOut.InGame = false;
            playerIn.InGame = true;

            // Update both players
            _teamMemberMatchRepository.Update(playerOut);
            _teamMemberMatchRepository.Update(playerIn);

            // Get match tracking for period and time information
            var matchTracking = _matchTrackingRepository.GetByMatchId(request.MatchId);
            if (matchTracking == null)
            {
                return Result<PlayerSubstitutionResponse>.Failure("Match tracking not found");
            }

            // Create personal events for substitution
            var substitutionTime = DateTime.UtcNow;
            
            // Calculate remaining time in current period for event tracking
            var periodTime = PeriodTimeCalculator.CalculateRemainingPeriodTime(matchTracking);
            
            // Event for player going out
            var playerOutEvent = new PersonalEvent
            {
                CreationTime = substitutionTime,
                Notes = "Player substituted out",
                Type = "substitution out",
                Period = matchTracking.CurrentPeriod,
                PeriodTime = periodTime,
                IdTeam = request.TeamId,
                IdPlayer = request.PlayerOutId,
                IdMatch = request.MatchId
            };
            
            // Event for player coming in
            var playerInEvent = new PersonalEvent
            {
                CreationTime = substitutionTime,
                Notes = "Player substituted in",
                Type = "substitution in",
                Period = matchTracking.CurrentPeriod,
                PeriodTime = periodTime,
                IdTeam = request.TeamId,
                IdPlayer = request.PlayerInId,
                IdMatch = request.MatchId
            };

            // Add events to context (don't save yet, will be saved with transaction)
            _personalEventRepository.Create(playerOutEvent);
            _personalEventRepository.Create(playerInEvent);

            // Save changes to trigger the database updates
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var response = new PlayerSubstitutionResponse
            {
                Success = true,
                Message = "Substitution completed successfully",
                MatchId = request.MatchId,
                TeamId = request.TeamId,
                PlayerInName = $"Player {request.PlayerInId}", // This should be fetched from player data
                PlayerOutName = $"Player {request.PlayerOutId}", // This should be fetched from player data
                SubstitutionTime = DateTime.UtcNow
            };

            return Result<PlayerSubstitutionResponse>.Success(response);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<PlayerSubstitutionResponse>.Failure($"Error performing substitution: {ex.Message}");
        }
    }
}