namespace ticket_service.src.Tickets.Core.Application.Features.Graph.IndividualTickets.DeleteTicket;

public class DeleteGraphIndividualTicketResponse
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;

    public DeleteGraphIndividualTicketResponse(int id, string message)
    {
        Id = id;
        Message = message;
    }
}