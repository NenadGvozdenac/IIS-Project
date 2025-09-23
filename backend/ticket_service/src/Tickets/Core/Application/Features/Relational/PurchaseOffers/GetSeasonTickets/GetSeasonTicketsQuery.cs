using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.PurchaseOffers.GetSeasonTickets;

public record GetSeasonTicketsQuery() : IRequest<Result<IEnumerable<GetSeasonTicketsResponse>>>;
