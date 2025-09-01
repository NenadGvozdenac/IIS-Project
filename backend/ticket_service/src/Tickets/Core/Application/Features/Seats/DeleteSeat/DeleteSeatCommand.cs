using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Seats.DeleteSeat;

public class DeleteSeatCommand : IRequest<Result<DeleteSeatResponse>>
{
    public int IdSeat { get; set; }

    public DeleteSeatCommand(int idSeat)
    {
        IdSeat = idSeat;
    }
}
