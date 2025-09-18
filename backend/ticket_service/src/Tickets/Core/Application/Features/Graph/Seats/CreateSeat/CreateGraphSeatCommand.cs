using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.CreateSeat;

public class CreateGraphSeatCommand : IRequest<Result<CreateGraphSeatResponse>>
{
    public string Name { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Number { get; set; }
    public string Direction { get; set; } = string.Empty;

    public CreateGraphSeatCommand(string name, int row, int number, string direction)
    {
        Name = name;
        Row = row;
        Number = number;
        Direction = direction;
    }
}