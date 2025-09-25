using MassTransit;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Application.Saga.Messages;

namespace match_service.src.Matches.Core.Application.Saga.Consumers;

public class DeleteInfluxEventsConsumer : IConsumer<DeleteInfluxEventsCommand>
{
    private readonly IChronologicalEventInfluxRepository _influxRepository;

    public DeleteInfluxEventsConsumer(IChronologicalEventInfluxRepository influxRepository)
    {
        _influxRepository = influxRepository;
    }

    public async Task Consume(ConsumeContext<DeleteInfluxEventsCommand> context)
    {
        try
        {
            var playerId = context.Message.PlayerId;
            var teamId = context.Message.TeamId;

            var eventsDeleted = await _influxRepository.DeleteEventsByPlayerAndTeamAsync(playerId, teamId);
            Console.WriteLine($"[SAGA] Deleted {eventsDeleted} InfluxDB events for Player {playerId}, Team {teamId}");

            await context.Publish(new InfluxEventsDeletedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                PlayerId = playerId,
                TeamId = teamId,
                EventsDeleted = eventsDeleted
            });
        }
        catch (Exception ex)
        {
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