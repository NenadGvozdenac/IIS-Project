namespace ticket_service.src.Tickets.Core.Domain.Entities.Graph;

public class BoughtRelationship
{
    public int CustomerId { get; set; }
    public int IndividualTicketId { get; set; }
    public DateTime PurchasedAt { get; set; }
    public decimal Price { get; set; }
    public string RelationshipElementId { get; set; } = string.Empty;
}