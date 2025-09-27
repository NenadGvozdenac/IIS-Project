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
        Event(() => MatchDataRolledBack, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => TravelDataRolledBack, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => InfluxEventsRolledBack, x => x.CorrelateById(context => context.Message.CorrelationId));

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
                .Then(context =>
                {
                    Console.WriteLine($"Saga: Match data deleted, marking as completed");
                    context.Saga.MatchDataDeleted = true;
                    context.Saga.DeletedTeamMemberMatchCount = context.Message.TeamMemberMatchesDeleted;
                    context.Saga.DeletedPersonalEventCount = context.Message.PersonalEventsDeleted;
                    context.Saga.DeletedMatchDataBackup = $"PlayerId:{context.Saga.PlayerId},TeamId:{context.Saga.TeamId},TMM:{context.Message.TeamMemberMatchesDeleted},PE:{context.Message.PersonalEventsDeleted}";
                    
                    // ČUVAMO JSON BACKUP PODATKE ZA PRAVI ROLLBACK
                    context.Saga.TeamMemberMatchesJsonBackup = context.Message.TeamMemberMatchesBackup;
                    context.Saga.PersonalEventsJsonBackup = context.Message.PersonalEventsBackup;
                })
                .SendAsync(new Uri("queue:delete-travel-data"), context => context.Init<DeleteTravelDataCommand>(new
                {
                    CorrelationId = context.Saga.CorrelationId,
                    PlayerId = context.Saga.PlayerId,
                    TeamId = context.Saga.TeamId,
                    ForceError = context.Saga.PlayerId == 999 // 🔥 TEST: Forsira grešku za PlayerId 999
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
                .Then(context =>
                {
                    Console.WriteLine($"Saga: Travel data deleted, marking as completed");
                    context.Saga.TravelDataDeleted = true;
                    context.Saga.DeletedTravelInfoCount = context.Message.TravelInformationDeleted;
                    context.Saga.DeletedVisaCount = context.Message.VisasDeleted;
                    context.Saga.DeletedRequestCount = context.Message.TeamMemberRequestsDeleted ? 1 : 0;
                    context.Saga.DeletedTravelDataBackup = $"PlayerId:{context.Saga.PlayerId},TeamId:{context.Saga.TeamId},TI:{context.Message.TravelInformationDeleted},V:{context.Message.VisasDeleted},R:{(context.Message.TeamMemberRequestsDeleted ? 1 : 0)}";
                    
                    // ČUVAMO JSON BACKUP PODATKE ZA TRAVEL SERVICE
                    context.Saga.TravelInformationJsonBackup = context.Message.TravelInformationBackup;
                    context.Saga.VisasJsonBackup = context.Message.VisasBackup;
                    context.Saga.RequestsJsonBackup = context.Message.RequestsBackup;
                    context.Saga.TeamMemberRequestsJsonBackup = context.Message.TeamMemberRequestsBackup;
                })
                .SendAsync(new Uri("queue:delete-influx-events"), context => context.Init<DeleteInfluxEventsCommand>(new
                {
                    CorrelationId = context.Saga.CorrelationId,
                    PlayerId = context.Saga.PlayerId,
                    TeamId = context.Saga.TeamId
                }))
                .TransitionTo(DeletingInfluxEvents),
            When(TravelDataDeleteFailed)
                .Then(context =>
                {
                    Console.WriteLine($"Saga: Travel data deletion FAILED - starting rollback process");
                    
                    // ČUVAMO JSON BACKUP PODATKE IZ TravelDataDeleteFailedEvent
                    context.Saga.TravelInformationJsonBackup = context.Message.TravelInformationBackup;
                    context.Saga.VisasJsonBackup = context.Message.VisasBackup;
                    context.Saga.RequestsJsonBackup = context.Message.RequestsBackup;
                    context.Saga.TeamMemberRequestsJsonBackup = context.Message.TeamMemberRequestsBackup;
                })
                .PublishAsync(context => context.Init<RollbackTravelDataCommand>(new
                {
                    CorrelationId = context.Saga.CorrelationId,
                    BackupData = $"PlayerId:{context.Saga.PlayerId},TeamId:{context.Saga.TeamId}",
                    // PROSLEĐUJEMO JSON BACKUP PODATKE
                    TravelInformationBackup = context.Saga.TravelInformationJsonBackup,
                    VisasBackup = context.Saga.VisasJsonBackup,
                    RequestsBackup = context.Saga.RequestsJsonBackup,
                    TeamMemberRequestsBackup = context.Saga.TeamMemberRequestsJsonBackup
                }))
                .TransitionTo(RollingBackTravelData)
        );

        During(DeletingInfluxEvents,
            When(InfluxEventsDeleted)
                .Then(context =>
                {
                    Console.WriteLine($"Saga: Influx events deleted, marking as completed");
                    context.Saga.InfluxEventsDeleted = true;
                    context.Saga.DeletedInfluxEventCount = context.Message.EventsDeleted;
                    context.Saga.DeletedInfluxEventsBackup = $"PlayerId:{context.Saga.PlayerId},TeamId:{context.Saga.TeamId},IE:{context.Message.EventsDeleted}";
                    
                    // ČUVAMO JSON BACKUP PODATKE ZA INFLUXDB
                    context.Saga.ChronologicalEventsJsonBackup = context.Message.ChronologicalEventsBackup;
                })
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
                .Then(context =>
                {
                    Console.WriteLine($"Saga: TeamMember deletion failed - starting rollback process");
                })
                .PublishAsync(context => context.Init<RollbackInfluxEventsCommand>(new
                {
                    CorrelationId = context.Saga.CorrelationId,
                    BackupData = $"PlayerId:{context.Saga.PlayerId},TeamId:{context.Saga.TeamId}",
                    // PROSLEĐUJEMO JSON BACKUP PODATKE
                    ChronologicalEventsBackup = context.Saga.ChronologicalEventsJsonBackup
                }))
                .TransitionTo(RollingBackInfluxEvents)
        );

        // Rollback states
        During(RollingBackInfluxEvents,
            When(InfluxEventsRolledBack)
                .PublishAsync(context => context.Init<RollbackTravelDataCommand>(new
                {
                    CorrelationId = context.Saga.CorrelationId,
                    BackupData = $"PlayerId:{context.Saga.PlayerId},TeamId:{context.Saga.TeamId}",
                    // PROSLEĐUJEMO JSON BACKUP PODATKE
                    TravelInformationBackup = context.Saga.TravelInformationJsonBackup,
                    VisasBackup = context.Saga.VisasJsonBackup,
                    RequestsBackup = context.Saga.RequestsJsonBackup,
                    TeamMemberRequestsBackup = context.Saga.TeamMemberRequestsJsonBackup
                }))
                .TransitionTo(RollingBackTravelData)
        );

        During(RollingBackTravelData,
            When(TravelDataRolledBack)
                .PublishAsync(context => context.Init<RollbackMatchDataCommand>(new
                {
                    CorrelationId = context.Saga.CorrelationId,
                    BackupData = $"PlayerId:{context.Saga.PlayerId},TeamId:{context.Saga.TeamId}",
                    // PROSLEĐUJEMO JSON BACKUP PODATKE
                    TeamMemberMatchesBackup = context.Saga.TeamMemberMatchesJsonBackup,
                    PersonalEventsBackup = context.Saga.PersonalEventsJsonBackup
                }))
                .TransitionTo(RollingBackMatchData)
        );

        During(RollingBackMatchData,
            When(MatchDataRolledBack)
                .PublishAsync(context => context.Init<TeamMemberSagaCompletedEvent>(new
                {
                    CorrelationId = context.Saga.CorrelationId,
                    Success = false,
                    ErrorMessage = "TeamMember deletion failed - all data rolled back successfully"
                }))
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }

    // States
    public State DeletingMatchData { get; private set; } = null!;
    public State DeletingTravelData { get; private set; } = null!;
    public State DeletingInfluxEvents { get; private set; } = null!;
    public State DeletingTeamMember { get; private set; } = null!;
    public State RollingBackInfluxEvents { get; private set; } = null!;
    public State RollingBackTravelData { get; private set; } = null!;
    public State RollingBackMatchData { get; private set; } = null!;

    // Events
    public Event<DeleteTeamMemberSagaStarted> DeleteTeamMemberSagaStarted { get; private set; } = null!;
    public Event<MatchDataDeletedEvent> MatchDataDeleted { get; private set; } = null!;
    public Event<MatchDataDeleteFailedEvent> MatchDataDeleteFailed { get; private set; } = null!;
    public Event<TravelDataDeletedEvent> TravelDataDeleted { get; private set; } = null!;
    public Event<TravelDataDeleteFailedEvent> TravelDataDeleteFailed { get; private set; } = null!;
    public Event<InfluxEventsDeletedEvent> InfluxEventsDeleted { get; private set; } = null!;
    public Event<InfluxEventsDeleteFailedEvent> InfluxEventsDeleteFailed { get; private set; } = null!;
    public Event<TeamMemberDeletedEvent> TeamMemberDeleted { get; private set; } = null!;
    public Event<TeamMemberDeleteFailedEvent> TeamMemberDeleteFailed { get; private set; } = null!;
    public Event<MatchDataRolledBackEvent> MatchDataRolledBack { get; private set; } = null!;
    public Event<TravelDataRolledBackEvent> TravelDataRolledBack { get; private set; } = null!;
    public Event<InfluxEventsRolledBackEvent> InfluxEventsRolledBack { get; private set; } = null!;
}

public class DeleteTeamMemberSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = "Initial";
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    public string RequesterId { get; set; } = string.Empty;
    
    // Backup podaci za rollback - STRING FORMAT ZADRŽAVAM ZA KOMPATIBILNOST
    public string? DeletedMatchDataBackup { get; set; }
    public string? DeletedTravelDataBackup { get; set; }
    public string? DeletedInfluxEventsBackup { get; set; }
    
    // JSON BACKUP PODACI ZA PRAVI ROLLBACK
    public string? TeamMemberMatchesJsonBackup { get; set; }
    public string? PersonalEventsJsonBackup { get; set; }
    
    // TRAVEL SERVICE JSON BACKUP PODACI
    public string? TravelInformationJsonBackup { get; set; }
    public string? VisasJsonBackup { get; set; }
    public string? RequestsJsonBackup { get; set; }
    public string? TeamMemberRequestsJsonBackup { get; set; }
    
    // INFLUXDB JSON BACKUP PODACI
    public string? ChronologicalEventsJsonBackup { get; set; }
    
    // Flags za praćenje obrisanih podataka
    public bool MatchDataDeleted { get; set; }
    public bool TravelDataDeleted { get; set; }
    public bool InfluxEventsDeleted { get; set; }
    
    // Brojevi obrisanih zapisa za rollback validaciju
    public int DeletedTeamMemberMatchCount { get; set; }
    public int DeletedPersonalEventCount { get; set; }
    public int DeletedTravelInfoCount { get; set; }
    public int DeletedVisaCount { get; set; }
    public int DeletedRequestCount { get; set; }
    public int DeletedInfluxEventCount { get; set; }
}