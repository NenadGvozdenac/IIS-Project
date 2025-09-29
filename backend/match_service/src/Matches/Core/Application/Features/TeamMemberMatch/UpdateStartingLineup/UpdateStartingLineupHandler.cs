using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace match_service.src.Matches.Core.Application.Features.TeamMemberMatch.UpdateStartingLineup;

public class UpdateStartingLineupHandler : IRequestHandler<UpdateStartingLineupCommand, Result<UpdateStartingLineupResponse>>
{
    private readonly MatchDbContext _context;
    private readonly ITeamMemberMatchRepository _teamMemberMatchRepository;

    public UpdateStartingLineupHandler(MatchDbContext context, ITeamMemberMatchRepository teamMemberMatchRepository)
    {
        _context = context;
        _teamMemberMatchRepository = teamMemberMatchRepository;
    }

    public async Task<Result<UpdateStartingLineupResponse>> Handle(UpdateStartingLineupCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            int updatedCount = 0;

            foreach (var playerRequest in request.Players)
            {
                var existingTeamMemberMatch = _teamMemberMatchRepository.GetById(
                    request.MatchId, 
                    playerRequest.TeamId, 
                    playerRequest.PlayerId);

                if (existingTeamMemberMatch != null)
                {
                    existingTeamMemberMatch.StartingLineup = playerRequest.StartingLineup;
                    existingTeamMemberMatch.InGame = playerRequest.InGame;
                    
                    _teamMemberMatchRepository.Update(existingTeamMemberMatch);
                    updatedCount++;
                }
                Console.WriteLine($"Processed player {playerRequest.PlayerId} for team {playerRequest.TeamId}: " +
                                  $"StartingLineup={playerRequest.StartingLineup}, InGame={playerRequest.InGame}");
                Console.WriteLine(existingTeamMemberMatch == null
                    ? "No existing TeamMemberMatch found."
                    : $"Existing TeamMemberMatch found: StartingLineup={existingTeamMemberMatch.StartingLineup}, InGame={existingTeamMemberMatch.InGame}");                 
            }

            // Save all changes in transaction
            await _context.SaveChangesAsync(cancellationToken);
            
            // Commit transaction
            await transaction.CommitAsync(cancellationToken);

            var response = new UpdateStartingLineupResponse(request.MatchId, updatedCount);
            return Result<UpdateStartingLineupResponse>.Success(response);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<UpdateStartingLineupResponse>.Failure($"Failed to update starting lineup: {ex.Message}");
        }
    }
}
