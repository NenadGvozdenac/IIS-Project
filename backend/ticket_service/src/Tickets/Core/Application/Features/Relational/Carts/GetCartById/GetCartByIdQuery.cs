using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Carts.GetCartById;

public class GetCartByIdQuery : IRequest<Result<GetCartByIdResponse>>
{
    public int IdCart { get; set; }

    public GetCartByIdQuery(int idCart)
    {
        IdCart = idCart;
    }
}
