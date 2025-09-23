using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.DeleteCustomer;

public class DeleteGraphCustomerCommand : IRequest<Result<DeleteGraphCustomerResponse>>
{
    public int Id { get; set; }

    public DeleteGraphCustomerCommand(int id)
    {
        Id = id;
    }
}