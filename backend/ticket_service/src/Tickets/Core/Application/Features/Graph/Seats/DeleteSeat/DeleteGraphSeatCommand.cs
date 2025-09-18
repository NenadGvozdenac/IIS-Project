using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.DeleteSeat;

public class DeleteGraphSeatCommand : IRequest<Result<DeleteGraphSeatResponse>>
{
    public int Id { get; set; }

    public DeleteGraphSeatCommand(int id)
    {
        Id = id;
    }
}