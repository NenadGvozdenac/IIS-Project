using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.GetCustomerById;

public class GetGraphCustomerByIdQuery : IRequest<Result<GetGraphCustomerByIdResponse>>
{
    public int Id { get; set; }

    public GetGraphCustomerByIdQuery(int id)
    {
        Id = id;
    }
}