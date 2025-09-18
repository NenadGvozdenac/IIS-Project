namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.DeleteCustomer;

public class DeleteGraphCustomerResponse
{
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public DeleteGraphCustomerResponse(string email, string message)
    {
        Email = email;
        Message = message;
    }
}