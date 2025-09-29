using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.CreateCustomer;

public class CreateGraphCustomerCommand : IRequest<Result<CreateGraphCustomerResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;

    public CreateGraphCustomerCommand(string email, string name, string surname, string phone, string type)
    {
        Email = email;
        Name = name;
        Surname = surname;
        Phone = phone;
        Type = type;
    }
}