using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.GetSeatsByDirection;

public class GetGraphSeatsByDirectionQuery : IRequest<Result<List<GetGraphSeatsByDirectionResponse>>>
{
    public string Direction { get; set; } = string.Empty;

    public GetGraphSeatsByDirectionQuery(string direction)
    {
        Direction = direction;
    }
}