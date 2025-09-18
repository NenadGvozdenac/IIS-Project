namespace ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

public interface IGraphComplexQueryRepository
{
    Task<List<MatchRevenueDto>> GetMatchesWithRevenueAboveThreshold(int minTicketsSold);
    Task<List<CustomerSpendingDto>> GetCustomersWithHighSpending(decimal minSpending);
    Task<SectorSalesResultDto> GetSectorSalesForMatch(int matchId, int minTicketsSold);
    Task<List<CustomerMatchesDto>> GetCustomersWithMultipleMatches();
    Task<List<MatchAveragePriceDto>> GetMatchesWithAveragePrice(int minTicketsSold);
}

public class MatchRevenueDto
{
    public string MatchName { get; set; } = string.Empty;
    public int TicketsSold { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class CustomerSpendingDto
{
    public string CustomerName { get; set; } = string.Empty;
    public int TicketCount { get; set; }
    public decimal TotalSpent { get; set; }
}

public class SectorSalesDto
{
    public string Sector { get; set; } = string.Empty;
    public int TicketsSold { get; set; }
}

public class CustomerMatchesDto
{
    public string CustomerName { get; set; } = string.Empty;
    public int MatchCount { get; set; }
    public List<string> Matches { get; set; } = new List<string>();
}

public class MatchAveragePriceDto
{
    public string MatchName { get; set; } = string.Empty;
    public int TicketsSold { get; set; }
    public decimal AveragePrice { get; set; }
}

public class SectorSalesResultDto
{
    public string MatchName { get; set; } = string.Empty;
    public List<SectorSalesDto> Sectors { get; set; } = new List<SectorSalesDto>();
}