using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Zones.UpdateZone;

public class UpdateZoneCommand : IRequest<Result<UpdateZoneResponse>>
{
    public int IdZone { get; set; }
    public string? Name { get; set; }
    public int? Rank { get; set; }
    public int? MaximumCapacity { get; set; }
    public string? Status { get; set; }
}
