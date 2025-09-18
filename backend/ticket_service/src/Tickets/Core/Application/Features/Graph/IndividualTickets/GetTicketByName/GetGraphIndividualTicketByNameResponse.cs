namespace ticket_service.src.Tickets.Core.Application.Features.Graph.IndividualTickets.GetTicketByName;

public class GetGraphIndividualTicketByNameResponse
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime ReleasedAt { get; set; }
    public decimal Price { get; set; }

    public GetGraphIndividualTicketByNameResponse(string name, string description, string type, DateTime releasedAt, decimal price)
    {
        Name = name;
        Description = description;
        Type = type;
        ReleasedAt = releasedAt;
        Price = price;
    }
}