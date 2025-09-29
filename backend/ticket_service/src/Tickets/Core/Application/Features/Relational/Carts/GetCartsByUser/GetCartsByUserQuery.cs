using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Carts.GetCartsByUser;

public class GetCartsByUserQuery : IRequest<Result<List<GetCartsByUserResponse>>>
{
    public int IdUser { get; set; }
}
