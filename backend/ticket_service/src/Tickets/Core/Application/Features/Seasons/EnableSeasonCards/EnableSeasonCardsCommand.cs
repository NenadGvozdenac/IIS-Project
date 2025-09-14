using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Seasons.EnableSeasonCards;

public class EnableSeasonTicketsCommand : IRequest<Result<EnableSeasonTicketsResponse>>
{
    public int IdSeason { get; set; }
    public List<ZonePricing> ZonePrices { get; set; } = new List<ZonePricing>();
}

public class ZonePricing
{
    public int ZoneId { get; set; }
    public int Price { get; set; } // Price in cents
}