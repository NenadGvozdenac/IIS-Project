using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Relationships.IsForSeat.CreateIsForSeat;

public class CreateIsForSeatRelationshipCommand : IRequest<Result<CreateIsForSeatRelationshipResponse>>
{
    public int IndividualTicketId { get; set; }
    public int SeatId { get; set; }

    public CreateIsForSeatRelationshipCommand(int individualTicketId, int seatId)
    {
        IndividualTicketId = individualTicketId;
        SeatId = seatId;
    }
}