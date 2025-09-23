namespace ticket_service.src.Tickets.Core.Application.Features.Graph.IndividualTickets.CreateTicket;

public class CreateGraphIndividualTicketResponse
{
    public int Id { get; set; }
    public string ElementId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime ReleasedAt { get; set; }
    public decimal Price { get; set; }

    public CreateGraphIndividualTicketResponse(int id, string elementId, string name, string description, string type, DateTime releasedAt, decimal price)
    {
        Id = id;
        ElementId = elementId;
        Name = name;
        Description = description;
        Type = type;
        ReleasedAt = releasedAt;
        Price = price;
    }
}