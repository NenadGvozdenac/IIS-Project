using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Seats.GetSeatsByZone;

public class GetSeatsByZoneQuery : IRequest<Result<List<GetSeatsByZoneResponse>>>
{
    public int ZoneId { get; set; }
    
    public GetSeatsByZoneQuery(int zoneId)
    {
        ZoneId = zoneId;
    }
}
