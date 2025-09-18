using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.DeleteSeat;

public class DeleteGraphSeatCommand : IRequest<Result<DeleteGraphSeatResponse>>
{
    public string Name { get; set; } = string.Empty;

    public DeleteGraphSeatCommand(string name)
    {
        Name = name;
    }
}