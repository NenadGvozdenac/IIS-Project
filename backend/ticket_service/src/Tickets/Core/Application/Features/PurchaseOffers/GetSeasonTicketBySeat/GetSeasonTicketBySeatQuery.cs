using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.GetSeasonTicketBySeat;

public record GetSeasonTicketBySeatQuery(int ZoneId, int Row, int Number, int SeasonId) : IRequest<Result<GetSeasonTicketBySeatResponse>>;
