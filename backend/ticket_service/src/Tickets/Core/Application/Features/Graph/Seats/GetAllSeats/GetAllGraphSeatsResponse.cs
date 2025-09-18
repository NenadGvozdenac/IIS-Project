namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.GetAllSeats;

public class GetAllGraphSeatsResponse
{
    public int Id { get; set; }
    public string ElementId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Number { get; set; }
    public string Direction { get; set; } = string.Empty;
}