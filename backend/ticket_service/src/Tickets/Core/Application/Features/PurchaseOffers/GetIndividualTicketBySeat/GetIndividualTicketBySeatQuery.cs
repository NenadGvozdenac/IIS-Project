using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.GetIndividualTicketBySeat;

public record GetIndividualTicketBySeatQuery(int ZoneId, int Row, int Number, string Direction, int MatchId) : IRequest<Result<GetIndividualTicketBySeatResponse>>;
