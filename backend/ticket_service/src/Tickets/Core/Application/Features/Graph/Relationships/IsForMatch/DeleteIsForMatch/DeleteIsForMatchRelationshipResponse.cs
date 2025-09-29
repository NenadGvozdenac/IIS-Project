namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Relationships.IsForMatch.DeleteIsForMatch;

public class DeleteIsForMatchRelationshipResponse
{
    public int IndividualTicketId { get; set; }
    public int MatchId { get; set; }
    public string Message { get; set; } = string.Empty;
}