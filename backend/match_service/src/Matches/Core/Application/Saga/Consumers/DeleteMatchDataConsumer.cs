using MassTransit;
using Microsoft.Extensions.Logging;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Application.Saga.Messages;

namespace match_service.src.Matches.Core.Application.Saga.Consumers;

public class DeleteMatchDataConsumer : IConsumer<DeleteMatchDataCommand>
{
    private readonly ITeamMemberMatchRepository _teamMemberMatchRepository;
    private readonly IPersonalEventRepository _personalEventRepository;
    private readonly ITeamMemberRepository _teamMemberRepository;
    private readonly ILogger<DeleteMatchDataConsumer> _logger;

    public DeleteMatchDataConsumer(
        ITeamMemberMatchRepository teamMemberMatchRepository,
        IPersonalEventRepository personalEventRepository,
        ITeamMemberRepository teamMemberRepository,
        ILogger<DeleteMatchDataConsumer> logger)
    {
        _teamMemberMatchRepository = teamMemberMatchRepository;
        _personalEventRepository = personalEventRepository;
        _logger = logger;
        _teamMemberRepository = teamMemberRepository;
    }

    public async Task Consume(ConsumeContext<DeleteMatchDataCommand> context)
    {
        _logger.LogInformation("DeleteMatchDataConsumer started for Player {PlayerId}, Team {TeamId}", context.Message.PlayerId, context.Message.TeamId);
        
        try
        {
            var playerId = context.Message.PlayerId;
            var teamId = context.Message.TeamId;

            _logger.LogInformation("Getting match data for backup...");
            // BACKUP PODATAKA PRE BRISANJA
            var teamMemberMatches = _teamMemberMatchRepository.GetByPlayerAndTeam(playerId, teamId);
            var personalEvents = _personalEventRepository.GetByPlayerAndTeam(playerId, teamId);

            _logger.LogInformation("Serializing {TMM} TeamMemberMatches and {PE} PersonalEvents to JSON...", teamMemberMatches?.Count ?? 0, personalEvents?.Count ?? 0);
            
            // Serialize backup data kao JSON sa ReferenceHandler.IgnoreCycles
            var jsonOptions = new System.Text.Json.JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                WriteIndented = false
            };
            
            var teamMemberMatchesJson = System.Text.Json.JsonSerializer.Serialize(teamMemberMatches, jsonOptions);
            var personalEventsJson = System.Text.Json.JsonSerializer.Serialize(personalEvents, jsonOptions);
            
            Console.WriteLine($"[SAGA] BACKING UP {teamMemberMatches?.Count ?? 0} TeamMemberMatch and {personalEvents?.Count ?? 0} PersonalEvent records");

            _logger.LogInformation("Deleting TeamMemberMatch records...");
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
                TeamMemberDeleted = false, // We didn't delete the team member yet
                // DODAJEMO BACKUP JSON PODATKE
                TeamMemberMatchesBackup = teamMemberMatchesJson,
                PersonalEventsBackup = personalEventsJson
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR in DeleteMatchDataConsumer: {ErrorMessage}", ex.Message);
            
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