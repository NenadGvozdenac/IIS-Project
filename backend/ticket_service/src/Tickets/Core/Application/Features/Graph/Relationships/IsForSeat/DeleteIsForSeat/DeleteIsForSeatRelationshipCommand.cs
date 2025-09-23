using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Relationships.IsForSeat.DeleteIsForSeat;

public class DeleteIsForSeatRelationshipCommand : IRequest<Result<DeleteIsForSeatRelationshipResponse>>
{
    public int IndividualTicketId { get; set; }
    public int SeatId { get; set; }

    public DeleteIsForSeatRelationshipCommand(int individualTicketId, int seatId)
    {
        IndividualTicketId = individualTicketId;
        SeatId = seatId;
    }
}