using System;

namespace match_service.src.Matches.Core.Application.Saga.Messages;

// Commands that travel_service receives
public class DeleteTravelDataCommand
{
    public Guid CorrelationId { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
}

// Events that travel_service publishes
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