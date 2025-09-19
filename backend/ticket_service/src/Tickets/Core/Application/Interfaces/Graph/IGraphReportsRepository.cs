namespace ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

public interface IGraphReportsRepository
{
    // Prosti izveštaji
    Task<List<MatchTicketSalesReportDto>> GetMatchTicketSalesReport();
    Task<List<CustomerSpendingReportDto>> GetCustomerSpendingReport();
    
    // Složeni izveštaj
    Task<SectorAnalysisReportDto> GetSectorAnalysisReport();
}

// DTOs za proste izveštaje
public class MatchTicketSalesReportDto
{
    public string MatchName { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public int TotalTicketsSold { get; set; }
    public decimal TotalRevenue { get; set; }
    public string Hall { get; set; } = string.Empty;
}

public class CustomerSpendingReportDto
{
    public string CustomerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int TotalTicketsPurchased { get; set; }
    public decimal TotalAmountSpent { get; set; }
    public int MatchesAttended { get; set; }
}

// DTOs za složeni izveštaj
public class SectorAnalysisReportDto
{
    public string ReportGeneratedAt { get; set; } = string.Empty;
    public List<SectorDetailDto> SectorDetails { get; set; } = new List<SectorDetailDto>();
    public SectorSummaryDto Summary { get; set; } = new SectorSummaryDto();
}

public class SectorDetailDto
{
    public string SectorName { get; set; } = string.Empty;
    public List<SectorMatchAnalysisDto> MatchAnalysis { get; set; } = new List<SectorMatchAnalysisDto>();
    public decimal AveragePrice { get; set; }
    public int TotalTicketsSold { get; set; }
    public decimal TotalRevenue { get; set; }
    public string PopularityRank { get; set; } = string.Empty;
}

public class SectorMatchAnalysisDto
{
    public string MatchName { get; set; } = string.Empty;
    public int TicketsSold { get; set; }
    public decimal AveragePrice { get; set; }
    public decimal TotalRevenue { get; set; }
    public double SalesPercentage { get; set; }
}

public class SectorSummaryDto
{
    public string MostPopularSector { get; set; } = string.Empty;
    public string HighestRevenueSector { get; set; } = string.Empty;
    public string HighestAveragePriceSector { get; set; } = string.Empty;
    public int TotalTicketsSoldAllSectors { get; set; }
    public decimal TotalRevenueAllSectors { get; set; }
}