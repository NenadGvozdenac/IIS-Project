namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Seasons.GetAllSeasons;

public class GetAllSeasonsResponse
{
    public int IdSeason { get; set; }
    public DateOnly StartedAt { get; set; }
    public DateOnly? EndedAt { get; set; }
    public string Name { get; set; } = null!;
    public int MatchesCount { get; set; }
    public int SeasonTicketsCount { get; set; }
    public bool IsActive { get; set; }
    public bool TicketsForSale { get; set; }
    public DateTime? TicketsWentOnSale { get; set; }
}
