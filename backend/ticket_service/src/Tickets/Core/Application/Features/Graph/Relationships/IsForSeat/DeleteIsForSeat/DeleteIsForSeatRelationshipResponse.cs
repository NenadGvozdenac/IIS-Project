namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Relationships.IsForSeat.DeleteIsForSeat;

public class DeleteIsForSeatRelationshipResponse
{
    public int IndividualTicketId { get; set; }
    public int SeatId { get; set; }
    public string Message { get; set; } = string.Empty;
}