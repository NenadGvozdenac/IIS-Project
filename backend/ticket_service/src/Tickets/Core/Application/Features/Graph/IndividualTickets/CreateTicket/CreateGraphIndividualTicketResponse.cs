namespace ticket_service.src.Tickets.Core.Application.Features.Graph.IndividualTickets.CreateTicket;

public class CreateGraphIndividualTicketResponse
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime ReleasedAt { get; set; }
    public decimal Price { get; set; }

    public CreateGraphIndividualTicketResponse(string name, string description, string type, DateTime releasedAt, decimal price)
    {
        Name = name;
        Description = description;
        Type = type;
        ReleasedAt = releasedAt;
        Price = price;
    }
}