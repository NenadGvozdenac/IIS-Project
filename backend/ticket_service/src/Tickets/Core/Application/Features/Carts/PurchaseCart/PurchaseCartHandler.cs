using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.Carts.PurchaseCart;

public class PurchaseCartHandler : IRequestHandler<PurchaseCartCommand, Result<PurchaseCartResponse>>
{
    private readonly ICartRepository _cartRepository;
    private readonly ICreditCardRepository _creditCardRepository;
    private readonly IPurchaseOfferRepository _purchaseOfferRepository;

    public PurchaseCartHandler(ICartRepository cartRepository, ICreditCardRepository creditCardRepository, IPurchaseOfferRepository purchaseOfferRepository)
    {
        _cartRepository = cartRepository;
        _creditCardRepository = creditCardRepository;
        _purchaseOfferRepository = purchaseOfferRepository;
    }

    public Task<Result<PurchaseCartResponse>> Handle(PurchaseCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get the cart
            var cart = _cartRepository.GetById(request.IdCart);
            if (cart == null)
            {
                return Task.FromResult(Result<PurchaseCartResponse>.Failure("Cart not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            // Check if cart is already purchased
            if (cart.Status == "bought")
            {
                return Task.FromResult(Result<PurchaseCartResponse>.Failure("Cart has already been purchased")
                    .WithCode((int)ResultCode.BadRequest));
            }

            // Check if cart is empty
            if (cart.ItemsNumber == 0)
            {
                return Task.FromResult(Result<PurchaseCartResponse>.Failure("Cart is empty")
                    .WithCode((int)ResultCode.BadRequest));
            }

            // Verify credit card exists
            var creditCard = _creditCardRepository.GetById(request.IdCreditCard);
            if (creditCard == null)
            {
                return Task.FromResult(Result<PurchaseCartResponse>.Failure("Credit card not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            // Verify credit card belongs to the same user as the cart
            if (creditCard.IdUser != cart.IdUser)
            {
                return Task.FromResult(Result<PurchaseCartResponse>.Failure("Credit card does not belong to the cart owner")
                    .WithCode((int)ResultCode.Forbidden));
            }

            // Calculate total amount
            var totalAmount = cart.CartItems.Sum(ci => ci.Price);

            // Update cart status and credit card
            cart.Status = "bought";
            cart.IdCreditCard = request.IdCreditCard;
            cart.IsCurrent = false; // Set to false since it's now purchased

            // Update status of all purchase offers in the cart to "bought"
            foreach (var cartItem in cart.CartItems)
            {
                _purchaseOfferRepository.UpdateStatus(cartItem.IdPurchaseOffer, "bought");
            }

            _cartRepository.Update(cart);

            var response = new PurchaseCartResponse
            {
                IdCart = cart.IdCart,
                Status = cart.Status,
                IdCreditCard = request.IdCreditCard,
                PurchasedAt = DateOnly.FromDateTime(DateTime.Now),
                TotalAmount = totalAmount,
                Message = "Cart purchased successfully"
            };

            return Task.FromResult(Result<PurchaseCartResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<PurchaseCartResponse>.Failure($"An error occurred while purchasing the cart: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
