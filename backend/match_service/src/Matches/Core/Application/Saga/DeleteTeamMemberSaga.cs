using MassTransit;
using match_service.src.Matches.Core.Application.Saga.Messages;

namespace match_service.src.Matches.Core.Application.Saga;

public class DeleteTeamMemberSaga : MassTransitStateMachine<DeleteTeamMemberSagaState>
{
    public DeleteTeamMemberSaga()
    {
        InstanceState(x => x.CurrentState);
        
        Event(() => DeleteTeamMemberSagaStarted, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => MatchDataDeleted, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => MatchDataDeleteFailed, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => TravelDataDeleted, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => TravelDataDeleteFailed, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => InfluxEventsDeleted, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => InfluxEventsDeleteFailed, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => TeamMemberDeleted, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => TeamMemberDeleteFailed, x => x.CorrelateById(context => context.Message.CorrelationId));

        Initially(
            When(DeleteTeamMemberSagaStarted)
                .Then(context =>
                {
                    context.Saga.PlayerId = context.Message.PlayerId;
                    context.Saga.TeamId = context.Message.TeamId;
                    context.Saga.RequesterId = context.Message.RequesterId;
                })
                .SendAsync(new Uri("queue:delete-match-data"), context => context.Init<DeleteMatchDataCommand>(new
                {
                    CorrelationId = context.Saga.CorrelationId,
                    PlayerId = context.Saga.PlayerId,
                    TeamId = context.Saga.TeamId
                }))
                .TransitionTo(DeletingMatchData)
        );

        During(DeletingMatchData,
            When(MatchDataDeleted)
                .SendAsync(new Uri("queue:delete-travel-data"), context => context.Init<DeleteTravelDataCommand>(new
                {
                    CorrelationId = context.Saga.CorrelationId,
                    PlayerId = context.Saga.PlayerId,
                    TeamId = context.Saga.TeamId
                }))
                .TransitionTo(DeletingTravelData),
            When(MatchDataDeleteFailed)
                .PublishAsync(context => context.Init<TeamMemberSagaCompletedEvent>(new
                {
                    CorrelationId = context.Saga.CorrelationId,
                    Success = false,
                    ErrorMessage = context.Message.ErrorMessage
                }))
                .Finalize()
        );

        During(DeletingTravelData,
            When(TravelDataDeleted)
                .SendAsync(new Uri("queue:delete-influx-events"), context => context.Init<DeleteInfluxEventsCommand>(new
                {
                    CorrelationId = context.Saga.CorrelationId,
                    PlayerId = context.Saga.PlayerId,
                    TeamId = context.Saga.TeamId
                }))
                .TransitionTo(DeletingInfluxEvents),
            When(TravelDataDeleteFailed)
                .PublishAsync(context => context.Init<TeamMemberSagaCompletedEvent>(new
                {
                    CorrelationId = context.Saga.CorrelationId,
                    Success = false,
                    ErrorMessage = context.Message.ErrorMessage
                }))
                // TODO: Compensate by restoring deleted match data
                .Finalize()
        );

        During(DeletingInfluxEvents,
            When(InfluxEventsDeleted)
                .SendAsync(new Uri("queue:delete-team-member"), context => context.Init<DeleteTeamMemberCommand>(new
                {
                    CorrelationId = context.Saga.CorrelationId,
                    PlayerId = context.Saga.PlayerId,
                    TeamId = context.Saga.TeamId
                }))
                .TransitionTo(DeletingTeamMember),
            When(InfluxEventsDeleteFailed)
                .PublishAsync(context => context.Init<TeamMemberSagaCompletedEvent>(new
                {
                    CorrelationId = context.Saga.CorrelationId,
                    Success = false,
                    ErrorMessage = context.Message.ErrorMessage
                }))
                // TODO: Compensate by restoring deleted match and travel data
                .Finalize()
        );

        During(DeletingTeamMember,
            When(TeamMemberDeleted)
                .PublishAsync(context => context.Init<TeamMemberSagaCompletedEvent>(new
                {
                    CorrelationId = context.Saga.CorrelationId,
                    Success = true,
                    ErrorMessage = (string?)null
                }))
                .Finalize(),
            When(TeamMemberDeleteFailed)
                .PublishAsync(context => context.Init<TeamMemberSagaCompletedEvent>(new
                {
                    CorrelationId = context.Saga.CorrelationId,
                    Success = false,
                    ErrorMessage = context.Message.ErrorMessage
                }))
                // TODO: Compensate by restoring deleted match, travel and influx data
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }

    // States
    public State DeletingMatchData { get; private set; } = null!;
    public State DeletingTravelData { get; private set; } = null!;
    public State DeletingInfluxEvents { get; private set; } = null!;
    public State DeletingTeamMember { get; private set; } = null!;

    // Events
    public Event<DeleteTeamMemberSagaCommand> DeleteTeamMemberSagaStarted { get; private set; } = null!;
    public Event<MatchDataDeletedEvent> MatchDataDeleted { get; private set; } = null!;
    public Event<MatchDataDeleteFailedEvent> MatchDataDeleteFailed { get; private set; } = null!;
    public Event<TravelDataDeletedEvent> TravelDataDeleted { get; private set; } = null!;
    public Event<TravelDataDeleteFailedEvent> TravelDataDeleteFailed { get; private set; } = null!;
    public Event<InfluxEventsDeletedEvent> InfluxEventsDeleted { get; private set; } = null!;
    public Event<InfluxEventsDeleteFailedEvent> InfluxEventsDeleteFailed { get; private set; } = null!;
    public Event<TeamMemberDeletedEvent> TeamMemberDeleted { get; private set; } = null!;
    public Event<TeamMemberDeleteFailedEvent> TeamMemberDeleteFailed { get; private set; } = null!;
}

public class DeleteTeamMemberSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = "Initial";
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    public string RequesterId { get; set; } = string.Empty;
}