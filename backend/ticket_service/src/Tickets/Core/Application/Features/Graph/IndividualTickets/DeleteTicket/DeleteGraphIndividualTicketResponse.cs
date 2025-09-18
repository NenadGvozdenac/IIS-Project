namespace ticket_service.src.Tickets.Core.Application.Features.Graph.IndividualTickets.DeleteTicket;

public class DeleteGraphIndividualTicketResponse
{
    public string Name { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public DeleteGraphIndividualTicketResponse(string name, string message)
    {
        Name = name;
        Message = message;
    }
}