using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Carts.GetAllCarts;

public class GetAllCartsQuery : IRequest<Result<List<GetAllCartsResponse>>>
{
}
