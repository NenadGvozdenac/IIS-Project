using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.ComplexQueries.GetCustomerSpending;

public class GetCustomerSpendingResponse
{
    public List<CustomerSpendingDto> Customers { get; set; } = new List<CustomerSpendingDto>();
}