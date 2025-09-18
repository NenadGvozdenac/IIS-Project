using ticket_service.src.Tickets.Core.Domain.Entities.Graph;

namespace ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

public interface IGraphBoughtRelationshipRepository
{
    Task<BoughtRelationship?> CreateBoughtRelationship(int customerId, int individualTicketId, DateTime purchasedAt, decimal price);
    Task<List<BoughtRelationship>> GetBoughtRelationshipsByCustomerId(int customerId);
    Task<List<BoughtRelationship>> GetBoughtRelationshipsByTicketId(int individualTicketId);
    Task<BoughtRelationship?> GetBoughtRelationship(int customerId, int individualTicketId);
    Task<bool> IsTicketAlreadyBought(int individualTicketId);
    Task DeleteBoughtRelationship(int customerId, int individualTicketId);
}