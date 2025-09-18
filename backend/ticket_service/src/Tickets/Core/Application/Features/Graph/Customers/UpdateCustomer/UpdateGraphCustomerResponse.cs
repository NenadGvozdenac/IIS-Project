namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.UpdateCustomer;

public class UpdateGraphCustomerResponse
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;

    public UpdateGraphCustomerResponse(string email, string name, string surname, string phone, string type)
    {
        Email = email;
        Name = name;
        Surname = surname;
        Phone = phone;
        Type = type;
    }
}