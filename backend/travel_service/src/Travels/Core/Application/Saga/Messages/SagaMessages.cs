using System;

namespace match_service.src.Matches.Core.Application.Saga.Messages;

// Commands that travel_service receives
public class DeleteTravelDataCommand
{
    public Guid CorrelationId { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    public bool ForceError { get; set; } = false; // TEST PARAMETAR ZA ROLLBACK
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
    
    // JSON BACKUP PODACI ZA ROLLBACK
    public string? TravelInformationBackup { get; set; }
    public string? VisasBackup { get; set; }
    public string? RequestsBackup { get; set; }
    public string? TeamMemberRequestsBackup { get; set; }
}

public class TravelDataDeleteFailedEvent
{
    public Guid CorrelationId { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    
    // JSON BACKUP PODACI ZA ROLLBACK
    public string? TravelInformationBackup { get; set; }
    public string? VisasBackup { get; set; }
    public string? RequestsBackup { get; set; }
    public string? TeamMemberRequestsBackup { get; set; }
}

// Rollback Commands
public class RollbackTravelDataCommand
{
    public Guid CorrelationId { get; set; }
    public string BackupData { get; set; } = string.Empty;
    
    // JSON BACKUP PODACI ZA PRAVI ROLLBACK
    public string? TravelInformationBackup { get; set; }
    public string? VisasBackup { get; set; }
    public string? RequestsBackup { get; set; }
    public string? TeamMemberRequestsBackup { get; set; }
}

// Rollback Events
public class TravelDataRolledBackEvent
{
    public Guid CorrelationId { get; set; }
}