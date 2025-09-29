using MassTransit;
using Microsoft.Extensions.Logging;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Application.Saga.Messages;

namespace match_service.src.Matches.Core.Application.Saga.Consumers;

public class DeleteInfluxEventsConsumer : IConsumer<DeleteInfluxEventsCommand>
{
    private readonly IChronologicalEventInfluxRepository _influxRepository;
    private readonly ILogger<DeleteInfluxEventsConsumer> _logger;

    public DeleteInfluxEventsConsumer(IChronologicalEventInfluxRepository influxRepository, ILogger<DeleteInfluxEventsConsumer> logger)
    {
        _influxRepository = influxRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DeleteInfluxEventsCommand> context)
    {
        _logger.LogInformation("DeleteInfluxEventsConsumer started for Player {PlayerId}, Team {TeamId}", context.Message.PlayerId, context.Message.TeamId);
        
        try
        {
            var playerId = context.Message.PlayerId;
            var teamId = context.Message.TeamId;

            _logger.LogInformation("Getting InfluxDB events for backup...");
            // BACKUP INFLUXDB PODATAKA PRE BRISANJA
            var chronologicalEvents = await _influxRepository.GetEventsByPlayerAndTeamAsync(playerId, teamId);
            
            _logger.LogInformation("Serializing {Count} events to JSON...", chronologicalEvents.Count);
            
            // Serialize backup data kao JSON sa ReferenceHandler.IgnoreCycles
            var jsonOptions = new System.Text.Json.JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                WriteIndented = false
            };
            
            var chronologicalEventsJson = System.Text.Json.JsonSerializer.Serialize(chronologicalEvents, jsonOptions);
            
            Console.WriteLine($"[SAGA] BACKING UP {chronologicalEvents.Count} InfluxDB ChronologicalEvents");

            _logger.LogInformation("Deleting InfluxDB events...");
            var eventsDeleted = await _influxRepository.DeleteEventsByPlayerAndTeamAsync(playerId, teamId);
            Console.WriteLine($"[SAGA] Deleted {eventsDeleted} InfluxDB events for Player {playerId}, Team {teamId}");

            await context.Publish(new InfluxEventsDeletedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                PlayerId = playerId,
                TeamId = teamId,
                EventsDeleted = eventsDeleted,
                
                // DODAJEMO JSON BACKUP PODATKE
                ChronologicalEventsBackup = chronologicalEventsJson
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR in DeleteInfluxEventsConsumer: {ErrorMessage}", ex.Message);
            
            await context.Publish(new InfluxEventsDeleteFailedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                PlayerId = context.Message.PlayerId,
                TeamId = context.Message.TeamId,
                ErrorMessage = ex.Message
            });
        }
    }
}