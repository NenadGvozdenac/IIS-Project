using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.UpdateSeat;

public class UpdateGraphSeatCommand : IRequest<Result<UpdateGraphSeatResponse>>
{
    public string Name { get; set; } = string.Empty;
    public string NewName { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Number { get; set; }
    public string Direction { get; set; } = string.Empty;

    public UpdateGraphSeatCommand(string name, string newName, int row, int number, string direction)
    {
        Name = name;
        NewName = newName;
        Row = row;
        Number = number;
        Direction = direction;
    }
}