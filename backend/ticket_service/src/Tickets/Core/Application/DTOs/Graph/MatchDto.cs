namespace ticket_service.src.Tickets.Core.Application.DTOs.Graph;

// Match DTOs
public class CreateMatchDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public string Type { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Hall { get; set; } = string.Empty;
    public bool IsInOurHall { get; set; }
}

public class UpdateMatchDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public string Type { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Hall { get; set; } = string.Empty;
    public bool IsInOurHall { get; set; }
}

public class MatchResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string ElementId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public string Type { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Hall { get; set; } = string.Empty;
    public bool IsInOurHall { get; set; }
}