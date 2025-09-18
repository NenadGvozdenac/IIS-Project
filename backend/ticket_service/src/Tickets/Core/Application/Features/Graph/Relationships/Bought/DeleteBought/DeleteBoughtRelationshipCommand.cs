using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Relationships.Bought.DeleteBought;

public class DeleteBoughtRelationshipCommand : IRequest<Result<DeleteBoughtRelationshipResponse>>
{
    public int CustomerId { get; set; }
    public int IndividualTicketId { get; set; }

    public DeleteBoughtRelationshipCommand(int customerId, int individualTicketId)
    {
        CustomerId = customerId;
        IndividualTicketId = individualTicketId;
    }
}