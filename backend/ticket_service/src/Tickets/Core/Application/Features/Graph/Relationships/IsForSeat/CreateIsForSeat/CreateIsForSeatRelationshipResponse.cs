namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Relationships.IsForSeat.CreateIsForSeat;

public class CreateIsForSeatRelationshipResponse
{
    public int IndividualTicketId { get; set; }
    public int SeatId { get; set; }
    public string RelationshipElementId { get; set; } = string.Empty;
}