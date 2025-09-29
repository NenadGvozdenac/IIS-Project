using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Carts.PurchaseCart;

public class PurchaseCartCommand : IRequest<Result<PurchaseCartResponse>>
{
    public int IdCart { get; set; }
    public int IdCreditCard { get; set; }
}
