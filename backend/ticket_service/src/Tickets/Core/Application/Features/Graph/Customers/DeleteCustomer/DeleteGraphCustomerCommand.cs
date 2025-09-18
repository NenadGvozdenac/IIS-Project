using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.DeleteCustomer;

public class DeleteGraphCustomerCommand : IRequest<Result<DeleteGraphCustomerResponse>>
{
    public string Email { get; set; } = string.Empty;

    public DeleteGraphCustomerCommand(string email)
    {
        Email = email;
    }
}