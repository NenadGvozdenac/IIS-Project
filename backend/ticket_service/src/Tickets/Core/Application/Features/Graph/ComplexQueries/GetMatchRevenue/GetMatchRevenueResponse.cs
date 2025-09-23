using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.ComplexQueries.GetMatchRevenue;

public class GetMatchRevenueResponse
{
    public List<MatchRevenueDto> Matches { get; set; } = new List<MatchRevenueDto>();
}