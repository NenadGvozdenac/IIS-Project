namespace ticket_service.src.Tickets.Core.Application.Features.Statistics.GetMatchStatistics;

public class GetMatchStatisticsResponse
{
    public int MatchId { get; set; }
    public string MatchName { get; set; } = null!;
    public DateTime ScheduledAt { get; set; }
    public string Type { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Hall { get; set; } = null!;
    public bool IsInOurHall { get; set; }
    public DateTime? TicketsWentOnSale { get; set; }
    public string CompetitionName { get; set; } = null!;
    public string SeasonName { get; set; } = null!;
    public string TeamName { get; set; } = null!;
    public List<ZoneStatistics> Zones { get; set; } = new List<ZoneStatistics>();
    public decimal TotalRevenue { get; set; }
}

public class ZoneStatistics
{
    public int ZoneId { get; set; }
    public string ZoneName { get; set; } = null!;
    public int? PriceParameter { get; set; }
    public int? TimeParameter { get; set; }
    public int? MinimumSeatPrice { get; set; }
    public int? MaximumSeatPrice { get; set; }
    public int TotalTicketsSold { get; set; }
    public int TotalSeats { get; set; }
    public decimal TotalRevenue { get; set; }
}