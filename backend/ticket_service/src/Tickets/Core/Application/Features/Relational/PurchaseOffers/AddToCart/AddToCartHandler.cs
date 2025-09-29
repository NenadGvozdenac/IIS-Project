using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;
using ticket_service.src.Tickets.Core.Domain.Entities.Relational;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.PurchaseOffers.AddToCart;

public class AddToCartHandler : IRequestHandler<AddToCartCommand, Result<AddToCartResponse>>
{
    private readonly IPurchaseOfferRepository _purchaseOfferRepository;
    private readonly ICartRepository _cartRepository;
    private readonly ISeasonTicketRepository _seasonTicketRepository;
    private readonly IIndividualTicketRepository _individualTicketRepository;
    private readonly ITicketPriceCalculationService _priceCalculationService;

    public AddToCartHandler(
        IPurchaseOfferRepository purchaseOfferRepository,
        ICartRepository cartRepository,
        ISeasonTicketRepository seasonTicketRepository,
        IIndividualTicketRepository individualTicketRepository,
        ITicketPriceCalculationService priceCalculationService)
    {
        _purchaseOfferRepository = purchaseOfferRepository;
        _cartRepository = cartRepository;
        _seasonTicketRepository = seasonTicketRepository;
        _individualTicketRepository = individualTicketRepository;
        _priceCalculationService = priceCalculationService;
    }

    public Task<Result<AddToCartResponse>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Proveri da li purchase offer postoji i da li je dostupan
            var purchaseOffer = _purchaseOfferRepository.GetById(request.PurchaseOfferId);
            if (purchaseOffer == null)
            {
                return Task.FromResult(Result<AddToCartResponse>.Failure($"Purchase offer with ID {request.PurchaseOfferId} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            if (purchaseOffer.Type == "individual ticket")
            {
                var individualTicket = _individualTicketRepository.GetByPurchaseOfferId(request.PurchaseOfferId);

                if (individualTicket == null)
                {
                    return Task.FromResult(Result<AddToCartResponse>.Failure($"Individual ticket with Purchase Offer ID {request.PurchaseOfferId} not found")
                        .WithCode((int)ResultCode.NotFound));
                }

                if (individualTicket.IdMatchNavigation.TicketsForSale == false)
                {
                    return Task.FromResult(Result<AddToCartResponse>.Failure("Tickets for this match are not currently for sale")
                        .WithCode((int)ResultCode.BadRequest));
                }
            }

            if (purchaseOffer.Status != "enabled")
            {
                if (purchaseOffer.Status == "bought")
                {
                    return Task.FromResult(Result<AddToCartResponse>.Failure("This ticket has already been purchased")
                        .WithCode((int)ResultCode.BadRequest));
                }
                else
                {
                    return Task.FromResult(Result<AddToCartResponse>.Failure("This ticket is no longer available for purchase")
                        .WithCode((int)ResultCode.BadRequest));
                }
            }

            // 2. Pronađi trenutnu korpu korisnika
            var currentCart = _cartRepository.GetCurrentCartByUserId(request.UserId);
            if (currentCart == null)
            {
                return Task.FromResult(Result<AddToCartResponse>.Failure($"No active cart found for user {request.UserId}")
                    .WithCode((int)ResultCode.NotFound));
            }

            // 3. Proveri da li je stavka već u korpi
            if (currentCart.CartItems.Any(ci => ci.IdPurchaseOffer == request.PurchaseOfferId))
            {
                return Task.FromResult(Result<AddToCartResponse>.Failure("This ticket is already in your cart")
                    .WithCode((int)ResultCode.BadRequest));
            }

            // 4. Proveri konflikte sa sezonskim kartama i individualnim kartama
            var conflictValidation = ValidateTicketConflicts(purchaseOffer);
            if (!conflictValidation.Success)
            {
                return Task.FromResult(Result<AddToCartResponse>.Failure(conflictValidation.Message)
                    .WithCode((int)ResultCode.BadRequest));
            }

            // 5. Izračunaj cenu na osnovu tipa karte
            decimal price = 0;
            if (purchaseOffer.Type == "season ticket")
            {
                var seasonTicket = _seasonTicketRepository.GetByPurchaseOfferId(request.PurchaseOfferId);
                if (seasonTicket != null)
                {
                    price = seasonTicket.TicketPrice;
                }
            }
            else if (purchaseOffer.Type == "individual ticket")
            {
                // Pozovi PL/SQL funkciju za dinamičko izračunavanje cene individualne karte
                var individualTicket = _individualTicketRepository.GetByPurchaseOfferId(request.PurchaseOfferId);
                if (individualTicket != null)
                {
                    // Dohvati zonu iz sedišta
                    var seat = purchaseOffer.IdSeatNavigation;
                    if (seat?.IdZone != null)
                    {
                        price = _priceCalculationService.CalculateTicketPrice(
                            individualTicket.IdMatch, 
                            seat.IdZone.Value);
                    }
                }
            }

            // 6. Dodaj u korpu
            var cartItem = new CartItem
            {
                IdCart = currentCart.IdCart,
                IdPurchaseOffer = request.PurchaseOfferId,
                AddedAt = DateOnly.FromDateTime(DateTime.Now),
                Price = price
            };

            _cartRepository.AddItemToCart(cartItem);

            // 7. Označi kartu kao rezervisanu (može se vratiti ako se korpa ne kupi)
            _purchaseOfferRepository.UpdateStatus(request.PurchaseOfferId, "disabled");

            var response = new AddToCartResponse
            {
                IdCart = currentCart.IdCart,
                IdPurchaseOffer = request.PurchaseOfferId,
                AddedAt = cartItem.AddedAt,
                Price = price,
                Message = "Ticket successfully added to cart"
            };

            return Task.FromResult(Result<AddToCartResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<AddToCartResponse>.Failure($"An error occurred while adding ticket to cart: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }

    private (bool Success, string Message) ValidateTicketConflicts(PurchaseOffer purchaseOffer)
    {
        try
        {
            // For now, we allow both individual and season tickets to be purchased
            // The valid_from logic in the database trigger will handle when season tickets become active
            // Individual tickets can be purchased even if season tickets exist - business logic will handle conflicts
            
            return (true, "");
        }
        catch (Exception ex)
        {
            return (false, $"Error validating ticket conflicts: {ex.Message}");
        }
    }

    private void DisableIndividualTicketsForSeasonTicket(PurchaseOffer seasonTicketOffer)
    {
        try
        {
            // Pronađi sezonsku kartu da dohvatiš sezonu
            var seasonTicket = _seasonTicketRepository.GetByPurchaseOfferId(seasonTicketOffer.IdPurchaseOffer);
            if (seasonTicket == null) return;

            // Pronađi sve individualne karte za isto mesto u istoj sezoni
            var individualTicketsForSeat = _purchaseOfferRepository.GetIndividualTicketsBySeat(seasonTicketOffer.IdSeat);

            foreach (var individualTicket in individualTicketsForSeat)
            {
                // Proveri da li je individualna karta u istoj sezoni
                var ticket = _individualTicketRepository.GetByPurchaseOfferId(individualTicket.IdPurchaseOffer);
                if (ticket?.IdMatchNavigation?.IdSeason == seasonTicket.IdSeason)
                {
                    // Onemogući individualnu kartu
                    _purchaseOfferRepository.UpdateStatus(individualTicket.IdPurchaseOffer, "disabled");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error disabling individual tickets: {ex.Message}");
        }
    }
}
