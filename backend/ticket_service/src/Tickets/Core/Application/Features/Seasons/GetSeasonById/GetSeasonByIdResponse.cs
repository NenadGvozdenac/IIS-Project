namespace ticket_service.src.Tickets.Core.Application.Features.Seasons.GetSeasonById;

public class GetSeasonByIdResponse
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
    public List<MatchResponse> Matches { get; set; } = new List<MatchResponse>();
    public List<SeasonTicketResponse> SeasonTickets { get; set; } = new List<SeasonTicketResponse>();
}

public class MatchResponse
{
    public int IdMatch { get; set; }
    public string Name { get; set; } = null!;
    public string? City { get; set; }
    public string? Hall { get; set; }
    public DateTime ScheduledAt { get; set; }
}

public class SeasonTicketResponse
{
    public int IdPurchaseOffer { get; set; }
    public decimal TicketPrice { get; set; }
}
