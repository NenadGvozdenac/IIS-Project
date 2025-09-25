using MassTransit;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Application.Saga.Messages;

namespace match_service.src.Matches.Core.Application.Saga.Consumers;

public class DeleteMatchDataConsumer : IConsumer<DeleteMatchDataCommand>
{
    private readonly ITeamMemberMatchRepository _teamMemberMatchRepository;
    private readonly IPersonalEventRepository _personalEventRepository;
    private readonly ITeamMemberRepository _teamMemberRepository;

    public DeleteMatchDataConsumer(
        ITeamMemberMatchRepository teamMemberMatchRepository,
        IPersonalEventRepository personalEventRepository,
        ITeamMemberRepository teamMemberRepository)
    {
        _teamMemberMatchRepository = teamMemberMatchRepository;
        _personalEventRepository = personalEventRepository;
        _teamMemberRepository = teamMemberRepository;
    }

    public async Task Consume(ConsumeContext<DeleteMatchDataCommand> context)
    {
        try
        {
            var playerId = context.Message.PlayerId;
            var teamId = context.Message.TeamId;

            // Delete TeamMemberMatch records
            var teamMemberMatchesDeleted = _teamMemberMatchRepository.DeleteByPlayerAndTeam(playerId, teamId);
            Console.WriteLine($"[SAGA] Deleted {teamMemberMatchesDeleted} TeamMemberMatch records for Player {playerId}, Team {teamId}");
            
            // Delete PersonalEvent records
            var personalEventsDeleted = _personalEventRepository.DeleteByPlayerAndTeam(playerId, teamId);
            Console.WriteLine($"[SAGA] Deleted {personalEventsDeleted} PersonalEvent records for Player {playerId}, Team {teamId}");
            
            // Note: We don't delete the TeamMember yet - this will be done after travel data is deleted

            await context.Publish(new MatchDataDeletedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                PlayerId = playerId,
                TeamId = teamId,
                TeamMemberMatchesDeleted = teamMemberMatchesDeleted,
                PersonalEventsDeleted = personalEventsDeleted,
                TeamMemberDeleted = false // We didn't delete the team member yet
            });
        }
        catch (Exception ex)
        {
            await context.Publish(new MatchDataDeleteFailedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                PlayerId = context.Message.PlayerId,
                TeamId = context.Message.TeamId,
                ErrorMessage = ex.Message
            });
        }
    }
}