using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.PurchaseOffers.RemoveFromCart;

public class RemoveFromCartHandler : IRequestHandler<RemoveFromCartCommand, Result<RemoveFromCartResponse>>
{
    private readonly ICartRepository _cartRepository;
    private readonly IPurchaseOfferRepository _purchaseOfferRepository;

    public RemoveFromCartHandler(ICartRepository cartRepository, IPurchaseOfferRepository purchaseOfferRepository)
    {
        _cartRepository = cartRepository;
        _purchaseOfferRepository = purchaseOfferRepository;
    }

    public Task<Result<RemoveFromCartResponse>> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Pronađi trenutnu korpu korisnika
            var currentCart = _cartRepository.GetCurrentCartByUserId(request.UserId);
            if (currentCart == null)
            {
                return Task.FromResult(Result<RemoveFromCartResponse>.Failure($"No active cart found for user {request.UserId}")
                    .WithCode((int)ResultCode.NotFound));
            }

            // Ukloni stavku iz korpe
            var success = _cartRepository.RemoveItemFromCart(currentCart.IdCart, request.PurchaseOfferId);
            
            if (!success)
            {
                return Task.FromResult(Result<RemoveFromCartResponse>.Failure("Item not found in cart")
                    .WithCode((int)ResultCode.NotFound));
            }

            // Vrati kartu u dostupno stanje
            _purchaseOfferRepository.UpdateStatus(request.PurchaseOfferId, "enabled");

            var response = new RemoveFromCartResponse
            {
                Success = true,
                Message = "Item successfully removed from cart"
            };

            return Task.FromResult(Result<RemoveFromCartResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<RemoveFromCartResponse>.Failure($"An error occurred while removing item from cart: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
