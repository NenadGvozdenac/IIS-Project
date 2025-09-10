using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.CheckSeasonTicketConflict;

public record CheckSeasonTicketConflictQuery(int SeatId, string MatchDate) : IRequest<Result<CheckSeasonTicketConflictResponse>>;
