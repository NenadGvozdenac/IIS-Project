namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.GetSeatById;

public class GetGraphSeatByIdResponse
{
    public int Id { get; set; }
    public string ElementId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Number { get; set; }
    public string Direction { get; set; } = string.Empty;

    public GetGraphSeatByIdResponse(int id, string elementId, string name, int row, int number, string direction)
    {
        Id = id;
        ElementId = elementId;
        Name = name;
        Row = row;
        Number = number;
        Direction = direction;
    }
}