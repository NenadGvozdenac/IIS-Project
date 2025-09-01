using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Seats.UpdateSeat;

public class UpdateSeatCommand : IRequest<Result<UpdateSeatResponse>>
{
    public int IdSeat { get; set; }
    public int? Row { get; set; }
    public int? Number { get; set; }
    public string? Type { get; set; }
    public string? Direction { get; set; }
    public string? Status { get; set; }
    public int? IdZone { get; set; }
}
