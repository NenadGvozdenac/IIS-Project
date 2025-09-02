using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.Carts.GetAllCarts;

public class GetAllCartsHandler : IRequestHandler<GetAllCartsQuery, Result<List<GetAllCartsResponse>>>
{
    private readonly ICartRepository _cartRepository;
    private readonly ICreditCardEncryptionService _encryptionService;

    public GetAllCartsHandler(ICartRepository cartRepository, ICreditCardEncryptionService encryptionService)
    {
        _cartRepository = cartRepository;
        _encryptionService = encryptionService;
    }

    public Task<Result<List<GetAllCartsResponse>>> Handle(GetAllCartsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var carts = _cartRepository.GetAll();

            var response = carts.Select(c => new GetAllCartsResponse
            {
                IdCart = c.IdCart,
                CreatedAt = c.CreatedAt,
                ItemsNumber = c.ItemsNumber,
                Status = c.Status,
                IsCurrent = c.IsCurrent,
                IdCreditCard = c.IdCreditCard,
                IdUser = c.IdUser,
                UserName = c.IdUserNavigation?.Name,
                UserEmail = c.IdUserNavigation?.Email,
                CreditCardNumber = c.IdCreditCardNavigation?.Number != null 
                    ? _encryptionService.MaskCardNumber(_encryptionService.DecryptCardNumber(c.IdCreditCardNavigation.Number))
                    : null,
                CartItems = c.CartItems.Select(ci => new CartItemResponse
                {
                    IdCart = ci.IdCart,
                    IdPurchaseOffer = ci.IdPurchaseOffer,
                    AddedAt = ci.AddedAt,
                    Price = ci.Price
                }).ToList()
            }).ToList();

            return Task.FromResult(Result<List<GetAllCartsResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<List<GetAllCartsResponse>>.Failure($"An error occurred while retrieving carts: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
