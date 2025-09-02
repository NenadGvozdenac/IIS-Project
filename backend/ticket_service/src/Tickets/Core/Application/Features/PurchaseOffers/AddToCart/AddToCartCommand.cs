using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.AddToCart;

public record AddToCartCommand(int PurchaseOfferId, int UserId) : IRequest<Result<AddToCartResponse>>;
