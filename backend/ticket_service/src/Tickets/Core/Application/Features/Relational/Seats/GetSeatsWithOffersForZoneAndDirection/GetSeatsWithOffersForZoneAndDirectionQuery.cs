using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Seats.GetSeatsWithOffersForZoneAndDirection;

public class GetSeatsWithOffersForZoneAndDirectionQuery : IRequest<Result<List<GetSeatsWithOffersForZoneAndDirectionResponse>>>
{
    public int ZoneId { get; }
    public string Direction { get; }
    public int MatchId { get; }

    public GetSeatsWithOffersForZoneAndDirectionQuery(int zoneId, string direction, int matchId)
    {
        ZoneId = zoneId;
        Direction = direction;
        MatchId = matchId;
    }
}