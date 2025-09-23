namespace ticket_service.src.Tickets.Core.Domain.Entities.Relational;

public class MatchSummaryReport
{
    public int MatchId { get; set; }
    public string MatchName { get; set; } = string.Empty;
    public DateTime MatchDate { get; set; }
    public string MatchType { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Hall { get; set; } = string.Empty;
    public string SeasonName { get; set; } = string.Empty;
    public string? CompetitionName { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public int TotalTicketsSold { get; set; }
    public decimal TotalRevenue { get; set; }
    public int VipZoneTickets { get; set; }
    public decimal VipZoneRevenue { get; set; }
    public int RegularZoneTickets { get; set; }
    public decimal RegularZoneRevenue { get; set; }
    public decimal AverageTicketPrice { get; set; }
    public decimal StadiumFillPercentage { get; set; }
    public string HighestSellingZone { get; set; } = string.Empty;
    public string LowestSellingZone { get; set; } = string.Empty;
    public string TrackingStatus { get; set; } = string.Empty;
    public int OurPoints { get; set; }
    public int OpponentPoints { get; set; }
}

public class MatchSummaryReportResponse
{
    public List<MatchSummaryReport> Matches { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

public class SeasonStatistics
{
    public string SeasonName { get; set; } = string.Empty;
    public int TotalMatches { get; set; }
    public long TotalTicketsSold { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageFillPercentage { get; set; }
    public string BestMatchName { get; set; } = string.Empty;
    public decimal BestMatchRevenue { get; set; }
    public string WorstMatchName { get; set; } = string.Empty;
    public decimal WorstMatchRevenue { get; set; }
}