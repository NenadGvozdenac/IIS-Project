using MassTransit;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Application.Saga.Messages;

namespace match_service.src.Matches.Core.Application.Saga.Consumers;

public class DeleteTeamMemberConsumer : IConsumer<DeleteTeamMemberCommand>
{
    private readonly ITeamMemberRepository _teamMemberRepository;

    public DeleteTeamMemberConsumer(ITeamMemberRepository teamMemberRepository)
    {
        _teamMemberRepository = teamMemberRepository;
    }

    public async Task Consume(ConsumeContext<DeleteTeamMemberCommand> context)
    {
        try
        {
            var playerId = context.Message.PlayerId;
            var teamId = context.Message.TeamId;

            // Delete the TeamMember itself (all dependencies should be resolved by now)
            var teamMemberDeleted = _teamMemberRepository.Delete(teamId, playerId);
            Console.WriteLine($"[SAGA] Deleted TeamMember record for Player {playerId}, Team {teamId}: {teamMemberDeleted}");

            await context.Publish(new TeamMemberDeletedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                PlayerId = playerId,
                TeamId = teamId,
                TeamMemberDeleted = teamMemberDeleted
            });
        }
        catch (Exception ex)
        {
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