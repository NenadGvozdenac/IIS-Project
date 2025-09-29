using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Reports.GetMatchTicketSalesReport;

public class GetMatchTicketSalesReportResponse
{
    public List<MatchTicketSalesReportDto> MatchSales { get; set; } = new List<MatchTicketSalesReportDto>();
    public string ReportGeneratedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    public int TotalMatches { get; set; }
    public int TotalTicketsSold { get; set; }
    public decimal TotalRevenue { get; set; }
}