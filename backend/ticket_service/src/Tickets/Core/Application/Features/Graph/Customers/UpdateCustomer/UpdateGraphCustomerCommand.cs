using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.UpdateCustomer;

public class UpdateGraphCustomerCommand : IRequest<Result<UpdateGraphCustomerResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string NewEmail { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;

    public UpdateGraphCustomerCommand(string email, string newEmail, string name, string surname, string phone, string type)
    {
        Email = email;
        NewEmail = newEmail;
        Name = name;
        Surname = surname;
        Phone = phone;
        Type = type;
    }
}