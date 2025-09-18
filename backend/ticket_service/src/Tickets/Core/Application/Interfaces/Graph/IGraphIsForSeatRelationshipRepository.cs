using ticket_service.src.Tickets.Core.Domain.Entities.Graph;

namespace ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

public interface IGraphIsForSeatRelationshipRepository
{
    Task<IsForSeatRelationship?> CreateIsForSeatRelationship(int individualTicketId, int seatId);
    Task<List<IsForSeatRelationship>> GetIsForSeatRelationshipsByTicketId(int individualTicketId);
    Task<List<IsForSeatRelationship>> GetIsForSeatRelationshipsBySeatId(int seatId);
    Task<IsForSeatRelationship?> GetIsForSeatRelationship(int individualTicketId, int seatId);
    Task DeleteIsForSeatRelationship(int individualTicketId, int seatId);
}