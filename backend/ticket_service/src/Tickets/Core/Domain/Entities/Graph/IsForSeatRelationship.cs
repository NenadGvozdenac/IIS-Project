namespace ticket_service.src.Tickets.Core.Domain.Entities.Graph;

public class IsForSeatRelationship
{
    public int IndividualTicketId { get; set; }
    public int SeatId { get; set; }
    public string RelationshipElementId { get; set; } = string.Empty;
}