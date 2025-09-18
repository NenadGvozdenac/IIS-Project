using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.UpdateSeat;

public class UpdateGraphSeatCommand : IRequest<Result<UpdateGraphSeatResponse>>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Number { get; set; }
    public string Direction { get; set; } = string.Empty;

    public UpdateGraphSeatCommand(int id, string name, int row, int number, string direction)
    {
        Id = id;
        Name = name;
        Row = row;
        Number = number;
        Direction = direction;
    }
}