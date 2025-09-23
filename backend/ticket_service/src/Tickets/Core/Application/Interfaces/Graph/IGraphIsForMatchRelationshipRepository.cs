using ticket_service.src.Tickets.Core.Domain.Entities.Graph;

namespace ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

public interface IGraphIsForMatchRelationshipRepository
{
    Task<IsForMatchRelationship?> CreateIsForMatchRelationship(int individualTicketId, int matchId);
    Task<List<IsForMatchRelationship>> GetIsForMatchRelationshipsByTicketId(int individualTicketId);
    Task<List<IsForMatchRelationship>> GetIsForMatchRelationshipsByMatchId(int matchId);
    Task<IsForMatchRelationship?> GetIsForMatchRelationship(int individualTicketId, int matchId);
    Task DeleteIsForMatchRelationship(int individualTicketId, int matchId);
}