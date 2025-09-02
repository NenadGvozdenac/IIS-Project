using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.GetSeasonTickets;

public record GetSeasonTicketsQuery() : IRequest<Result<IEnumerable<GetSeasonTicketsResponse>>>;
