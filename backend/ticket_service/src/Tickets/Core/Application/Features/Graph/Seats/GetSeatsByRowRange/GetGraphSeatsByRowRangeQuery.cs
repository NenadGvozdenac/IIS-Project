using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.GetSeatsByRowRange;

public class GetGraphSeatsByRowRangeQuery : IRequest<Result<List<GetGraphSeatsByRowRangeResponse>>>
{
    public int MinRow { get; set; }
    public int MaxRow { get; set; }

    public GetGraphSeatsByRowRangeQuery(int minRow, int maxRow)
    {
        MinRow = minRow;
        MaxRow = maxRow;
    }
}