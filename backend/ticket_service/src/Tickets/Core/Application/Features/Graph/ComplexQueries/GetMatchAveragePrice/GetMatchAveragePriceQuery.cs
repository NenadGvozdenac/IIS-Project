using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.ComplexQueries.GetMatchAveragePrice;

public class GetMatchAveragePriceQuery : IRequest<Result<GetMatchAveragePriceResponse>>
{
    public int MinTicketsSold { get; set; }

    public GetMatchAveragePriceQuery(int minTicketsSold = 1)
    {
        MinTicketsSold = minTicketsSold;
    }
}