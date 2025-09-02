using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Carts.GetCurrentCart;

public class GetCurrentCartQuery : IRequest<Result<GetCurrentCartResponse>>
{
    public int IdUser { get; set; }
}
