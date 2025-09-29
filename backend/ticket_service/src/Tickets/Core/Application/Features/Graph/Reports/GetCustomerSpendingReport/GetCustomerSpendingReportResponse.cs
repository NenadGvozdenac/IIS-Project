using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Reports.GetCustomerSpendingReport;

public class GetCustomerSpendingReportResponse
{
    public List<CustomerSpendingReportDto> CustomerSpending { get; set; } = new List<CustomerSpendingReportDto>();
    public string ReportGeneratedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    public int TotalCustomers { get; set; }
    public int TotalActiveCustomers { get; set; }
    public decimal TotalSpentByAllCustomers { get; set; }
    public decimal AverageSpendingPerCustomer { get; set; }
}