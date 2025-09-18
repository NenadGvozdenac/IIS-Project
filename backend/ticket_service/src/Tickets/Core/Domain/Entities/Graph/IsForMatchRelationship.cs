namespace ticket_service.src.Tickets.Core.Domain.Entities.Graph;

public class IsForMatchRelationship
{
    public int IndividualTicketId { get; set; }
    public int MatchId { get; set; }
    public string RelationshipElementId { get; set; } = string.Empty;
}