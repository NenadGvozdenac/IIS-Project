using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.Carts.GetCurrentCart;

public class GetCurrentCartHandler : IRequestHandler<GetCurrentCartQuery, Result<GetCurrentCartResponse>>
{
    private readonly ICartRepository _cartRepository;
    private readonly ICreditCardEncryptionService _encryptionService;

    public GetCurrentCartHandler(ICartRepository cartRepository, ICreditCardEncryptionService encryptionService)
    {
        _cartRepository = cartRepository;
        _encryptionService = encryptionService;
    }

    public Task<Result<GetCurrentCartResponse>> Handle(GetCurrentCartQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cart = _cartRepository.GetCurrentCartByUserId(request.IdUser);

            if (cart == null)
            {
                return Task.FromResult(Result<GetCurrentCartResponse>.Failure("No current cart found for user")
                    .WithCode((int)ResultCode.NotFound));
            }

            var totalAmount = cart.CartItems.Sum(ci => ci.Price);

            var response = new GetCurrentCartResponse
            {
                IdCart = cart.IdCart,
                CreatedAt = cart.CreatedAt,
                ItemsNumber = cart.ItemsNumber,
                Status = cart.Status,
                IsCurrent = cart.IsCurrent,
                IdCreditCard = cart.IdCreditCard,
                IdUser = cart.IdUser,
                UserName = cart.IdUserNavigation?.Name,
                UserEmail = cart.IdUserNavigation?.Email,
                CreditCardNumber = cart.IdCreditCardNavigation?.Number != null 
                    ? _encryptionService.MaskCardNumber(_encryptionService.DecryptCardNumber(cart.IdCreditCardNavigation.Number))
                    : null,
                TotalAmount = totalAmount,
                CartItems = cart.CartItems.Select(ci => new CartItemResponse
                {
                    IdCart = ci.IdCart,
                    IdPurchaseOffer = ci.IdPurchaseOffer,
                    AddedAt = ci.AddedAt,
                    Price = ci.Price,
                    PurchaseOfferName = ci.IdPurchaseOfferNavigation?.Name,
                    PurchaseOfferDescription = ci.IdPurchaseOfferNavigation?.Description
                }).ToList()
            };

            return Task.FromResult(Result<GetCurrentCartResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetCurrentCartResponse>.Failure($"An error occurred while retrieving current cart: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
