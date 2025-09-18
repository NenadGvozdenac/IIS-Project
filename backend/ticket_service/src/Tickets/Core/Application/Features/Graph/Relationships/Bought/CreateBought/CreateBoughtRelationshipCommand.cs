using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Relationships.Bought.CreateBought;

public class CreateBoughtRelationshipCommand : IRequest<Result<CreateBoughtRelationshipResponse>>
{
    public int CustomerId { get; set; }
    public int IndividualTicketId { get; set; }
    public DateTime PurchasedAt { get; set; }
    public decimal Price { get; set; }

    public CreateBoughtRelationshipCommand(int customerId, int individualTicketId, DateTime purchasedAt, decimal price)
    {
        CustomerId = customerId;
        IndividualTicketId = individualTicketId;
        PurchasedAt = purchasedAt;
        Price = price;
    }
}