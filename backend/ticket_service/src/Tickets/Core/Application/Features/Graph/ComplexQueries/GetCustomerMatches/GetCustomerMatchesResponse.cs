using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.ComplexQueries.GetCustomerMatches;

public class GetCustomerMatchesResponse
{
    public List<CustomerMatchesDto> Customers { get; set; } = new List<CustomerMatchesDto>();
}