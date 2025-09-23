using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.GetCustomerByEmail;

public class GetGraphCustomerByEmailQuery : IRequest<Result<GetGraphCustomerByEmailResponse>>
{
    public string Email { get; set; } = string.Empty;

    public GetGraphCustomerByEmailQuery(string email)
    {
        Email = email;
    }
}