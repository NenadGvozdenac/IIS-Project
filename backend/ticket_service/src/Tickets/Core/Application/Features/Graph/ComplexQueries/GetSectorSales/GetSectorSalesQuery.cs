using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.ComplexQueries.GetSectorSales;

public class GetSectorSalesQuery : IRequest<Result<GetSectorSalesResponse>>
{
    public int MatchId { get; set; }
    public int MinTicketsSold { get; set; }

    public GetSectorSalesQuery(int matchId, int minTicketsSold = 1)
    {
        MatchId = matchId;
        MinTicketsSold = minTicketsSold;
    }
}