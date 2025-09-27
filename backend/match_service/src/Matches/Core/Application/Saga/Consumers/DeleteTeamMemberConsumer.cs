using MassTransit;
using Microsoft.Extensions.Logging;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Application.Saga.Messages;

namespace match_service.src.Matches.Core.Application.Saga.Consumers;

public class DeleteTeamMemberConsumer : IConsumer<DeleteTeamMemberCommand>
{
    private readonly ITeamMemberRepository _teamMemberRepository;
    private readonly ILogger<DeleteTeamMemberConsumer> _logger;

    public DeleteTeamMemberConsumer(ITeamMemberRepository teamMemberRepository, ILogger<DeleteTeamMemberConsumer> logger)
    {
        _teamMemberRepository = teamMemberRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DeleteTeamMemberCommand> context)
    {
        _logger.LogInformation("DeleteTeamMemberConsumer started for Player {PlayerId}, Team {TeamId}", context.Message.PlayerId, context.Message.TeamId);
        
        try
        {
            var playerId = context.Message.PlayerId;
            var teamId = context.Message.TeamId;

            _logger.LogInformation("Attempting to delete TeamMember...");
            // Delete the TeamMember itself (all dependencies should be resolved by now)
            var teamMemberDeleted = _teamMemberRepository.Delete(playerId, teamId);
            Console.WriteLine($"[SAGA] Deleted TeamMember record for Player {playerId}, Team {teamId}: {teamMemberDeleted}");
            
            _logger.LogInformation("TeamMember deletion result: {Result}", teamMemberDeleted);

            // Check if TeamMember deletion was successful
            if (teamMemberDeleted)
            {
                await context.Publish(new TeamMemberDeletedEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    PlayerId = playerId,
                    TeamId = teamId,
                    TeamMemberDeleted = teamMemberDeleted
                });
            }
            else
            {
                Console.WriteLine($"[SAGA] TeamMember deletion failed - triggering rollback");
                await context.Publish(new TeamMemberDeleteFailedEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    PlayerId = playerId,
                    TeamId = teamId,
                    ErrorMessage = "TeamMember deletion failed - no record found or constraint violation"
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR in DeleteTeamMemberConsumer: {ErrorMessage}", ex.Message);
            
            await context.Publish(new TeamMemberDeleteFailedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                PlayerId = context.Message.PlayerId,
                TeamId = context.Message.TeamId,
                ErrorMessage = ex.Message
            });
        }
    }
}