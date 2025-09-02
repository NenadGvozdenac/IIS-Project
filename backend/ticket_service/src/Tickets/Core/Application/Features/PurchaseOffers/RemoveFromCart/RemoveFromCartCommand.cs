using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.RemoveFromCart;

public record RemoveFromCartCommand(int UserId, int PurchaseOfferId) : IRequest<Result<RemoveFromCartResponse>>;
