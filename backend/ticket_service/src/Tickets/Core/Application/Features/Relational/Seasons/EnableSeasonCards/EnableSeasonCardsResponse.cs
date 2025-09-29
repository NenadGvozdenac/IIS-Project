namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Seasons.EnableSeasonCards;

public class EnableSeasonTicketsResponse
{
    public int IdSeason { get; set; }
    public string Name { get; set; } = null!;
    public bool TicketsForSale { get; set; }
    public DateTime? TicketsWentOnSale { get; set; }
    public int CreatedTicketsCount { get; set; }
    public string Message { get; set; } = null!;
}