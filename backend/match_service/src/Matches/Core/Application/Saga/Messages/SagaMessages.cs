using System;

namespace match_service.src.Matches.Core.Application.Saga.Messages;

// Saga Commands
public class DeleteTeamMemberSagaCommand
{
    public Guid CorrelationId { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    public string RequesterId { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
}

public class DeleteMatchDataCommand  
{
    public Guid CorrelationId { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
}

public class DeleteTravelDataCommand
{
    public Guid CorrelationId { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
}

public class DeleteInfluxEventsCommand
{
    public Guid CorrelationId { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
}

public class DeleteTeamMemberCommand
{
    public Guid CorrelationId { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
}

// Saga Events
public class MatchDataDeletedEvent
{
    public Guid CorrelationId { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    public int TeamMemberMatchesDeleted { get; set; }
    public int PersonalEventsDeleted { get; set; }
    public bool TeamMemberDeleted { get; set; }
}

public class MatchDataDeleteFailedEvent
{
    public Guid CorrelationId { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

public class TravelDataDeletedEvent
{
    public Guid CorrelationId { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    public int TravelInformationDeleted { get; set; }
    public int VisasDeleted { get; set; }
    public bool TeamMemberRequestsDeleted { get; set; }
}

public class TravelDataDeleteFailedEvent
{
    public Guid CorrelationId { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

public class InfluxEventsDeletedEvent
{
    public Guid CorrelationId { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    public int EventsDeleted { get; set; }
}

public class InfluxEventsDeleteFailedEvent
{
    public Guid CorrelationId { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

public class TeamMemberDeletedEvent
{
    public Guid CorrelationId { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    public bool TeamMemberDeleted { get; set; }
}

public class TeamMemberDeleteFailedEvent
{
    public Guid CorrelationId { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

public class TeamMemberSagaCompletedEvent
{
    public Guid CorrelationId { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    public DateTime CompletedAt { get; set; }
}

public class TeamMemberDeletionFailedEvent
{
    public Guid SagaId { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime FailedAt { get; set; }
}