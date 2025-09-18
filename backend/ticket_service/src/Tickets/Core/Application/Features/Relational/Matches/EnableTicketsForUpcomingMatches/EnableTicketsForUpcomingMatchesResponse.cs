namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Matches.EnableTicketsForUpcomingMatches;

public class EnableTicketsForUpcomingMatchesResponse
{
    public int IdMatch { get; set; }
    public string Name { get; set; } = null!;
    public DateTime ScheduledAt { get; set; }
    public string Hall { get; set; } = null!;
    public bool WasAlreadyEnabled { get; set; }
    public DateTime TicketsEnabledAt { get; set; }
    public string Message { get; set; } = null!;
    public int CreatedTicketsCount { get; set; }
}