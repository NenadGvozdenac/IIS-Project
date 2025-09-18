namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Relationships.Bought.DeleteBought;

public class DeleteBoughtRelationshipResponse
{
    public int CustomerId { get; set; }
    public int IndividualTicketId { get; set; }
    public string Message { get; set; } = string.Empty;
}