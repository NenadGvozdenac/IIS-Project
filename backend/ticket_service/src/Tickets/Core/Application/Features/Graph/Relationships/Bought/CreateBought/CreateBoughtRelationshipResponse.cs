namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Relationships.Bought.CreateBought;

public class CreateBoughtRelationshipResponse
{
    public int CustomerId { get; set; }
    public int IndividualTicketId { get; set; }
    public string RelationshipElementId { get; set; } = string.Empty;
    public DateTime PurchasedAt { get; set; }
    public decimal Price { get; set; }
}