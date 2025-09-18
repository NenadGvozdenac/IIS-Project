using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Seats.GetSeatById;

public class GetSeatByIdQuery : IRequest<Result<GetSeatByIdResponse>>
{
    public int IdSeat { get; set; }

    public GetSeatByIdQuery(int idSeat)
    {
        IdSeat = idSeat;
    }
}
