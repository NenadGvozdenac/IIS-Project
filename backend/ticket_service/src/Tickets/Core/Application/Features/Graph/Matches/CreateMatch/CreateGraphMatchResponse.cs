namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Matches.CreateMatch;

public class CreateGraphMatchResponse
{
    public int Id { get; set; }
    public string ElementId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public string Type { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Hall { get; set; } = string.Empty;
    public bool IsInOurHall { get; set; }
}