using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.GetIndividualTickets;

public record GetIndividualTicketsQuery(int MatchId) : IRequest<Result<IEnumerable<GetIndividualTicketsResponse>>>;
