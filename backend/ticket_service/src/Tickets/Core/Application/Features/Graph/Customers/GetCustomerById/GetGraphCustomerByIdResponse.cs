namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.GetCustomerById;

public class GetGraphCustomerByIdResponse
{
    public int Id { get; set; }
    public string ElementId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;

    public GetGraphCustomerByIdResponse(int id, string elementId, string email, string name, string surname, string phone, string type)
    {
        Id = id;
        ElementId = elementId;
        Email = email;
        Name = name;
        Surname = surname;
        Phone = phone;
        Type = type;
    }
}