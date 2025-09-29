using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.GetSeatById;

public class GetGraphSeatByIdQuery : IRequest<Result<GetGraphSeatByIdResponse>>
{
    public int Id { get; set; }

    public GetGraphSeatByIdQuery(int id)
    {
        Id = id;
    }
}