using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.PurchaseOffers.GetExistingSeasonTickets;

public record GetExistingSeasonTicketsQuery(int ZoneId, int SeasonId) : IRequest<Result<IEnumerable<GetExistingSeasonTicketsResponse>>>;
