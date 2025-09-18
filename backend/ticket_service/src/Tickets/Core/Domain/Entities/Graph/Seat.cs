namespace ticket_service.src.Tickets.Core.Domain.Entities.Graph;

public class Seat
{
    public string Name { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Number { get; set; }
    public string Direction { get; set; } = string.Empty;
}