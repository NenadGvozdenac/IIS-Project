namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.GetCustomerByEmail;

public class GetGraphCustomerByEmailResponse
{
    public int Id { get; set; }
    public string ElementId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}