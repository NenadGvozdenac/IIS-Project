namespace ticket_service.src.Tickets.Core.Application.Features.Graph.IndividualTickets.GetAllIndividualTickets;

public class GetAllGraphIndividualTicketsResponse
{
    public string Id { get; set; } = string.Empty;
    public string ElementId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime ReleasedAt { get; set; }
    public decimal Price { get; set; }
}