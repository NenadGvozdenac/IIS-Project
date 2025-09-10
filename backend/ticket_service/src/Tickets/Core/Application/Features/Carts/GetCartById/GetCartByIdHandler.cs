using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.Carts.GetCartById;

public class GetCartByIdHandler : IRequestHandler<GetCartByIdQuery, Result<GetCartByIdResponse>>
{
    private readonly ICartRepository _cartRepository;
    private readonly ICreditCardEncryptionService _encryptionService;

    public GetCartByIdHandler(ICartRepository cartRepository, ICreditCardEncryptionService encryptionService)
    {
        _cartRepository = cartRepository;
        _encryptionService = encryptionService;
    }

    public Task<Result<GetCartByIdResponse>> Handle(GetCartByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cart = _cartRepository.GetById(request.IdCart);

            if (cart == null)
            {
                return Task.FromResult(Result<GetCartByIdResponse>.Failure("Cart not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var response = new GetCartByIdResponse
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
                CartItems = cart.CartItems.Select(ci => new CartItemResponse
                {
                    IdCart = ci.IdCart,
                    IdPurchaseOffer = ci.IdPurchaseOffer,
                    AddedAt = ci.AddedAt,
                    Price = ci.Price,
                    ValidFrom = ci.ValidFrom
                }).ToList()
            };

            return Task.FromResult(Result<GetCartByIdResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetCartByIdResponse>.Failure($"An error occurred while retrieving cart: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
