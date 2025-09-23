using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.ComplexQueries.GetMatchRevenue;

public class GetMatchRevenueQuery : IRequest<Result<GetMatchRevenueResponse>>
{
    public int MinTicketsSold { get; set; }

    public GetMatchRevenueQuery(int minTicketsSold = 2)
    {
        MinTicketsSold = minTicketsSold;
    }
}