namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.DeleteCustomer;

public class DeleteGraphCustomerResponse
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;

    public DeleteGraphCustomerResponse(int id, string message)
    {
        Id = id;
        Message = message;
    }
}