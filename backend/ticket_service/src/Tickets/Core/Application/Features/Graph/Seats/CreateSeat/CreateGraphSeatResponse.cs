namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.CreateSeat;

public class CreateGraphSeatResponse
{
    public string Name { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Number { get; set; }
    public string Direction { get; set; } = string.Empty;
}