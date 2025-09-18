using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.UpdateCustomer;

public class UpdateGraphCustomerCommand : IRequest<Result<UpdateGraphCustomerResponse>>
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;

    public UpdateGraphCustomerCommand(int id, string email, string name, string surname, string phone, string type)
    {
        Id = id;
        Email = email;
        Name = name;
        Surname = surname;
        Phone = phone;
        Type = type;
    }
}