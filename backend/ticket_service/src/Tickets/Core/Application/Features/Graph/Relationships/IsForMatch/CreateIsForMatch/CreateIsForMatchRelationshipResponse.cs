namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Relationships.IsForMatch.CreateIsForMatch;

public class CreateIsForMatchRelationshipResponse
{
    public int IndividualTicketId { get; set; }
    public int MatchId { get; set; }
    public string RelationshipElementId { get; set; } = string.Empty;
}