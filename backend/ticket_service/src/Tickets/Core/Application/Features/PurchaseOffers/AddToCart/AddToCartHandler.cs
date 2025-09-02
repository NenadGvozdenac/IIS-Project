using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;
using ticket_service.src.Tickets.Core.Domain.Entities;

namespace ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.AddToCart;

public class AddToCartHandler : IRequestHandler<AddToCartCommand, Result<AddToCartResponse>>
{
    private readonly IPurchaseOfferRepository _purchaseOfferRepository;
    private readonly ICartRepository _cartRepository;
    private readonly ISeasonTicketRepository _seasonTicketRepository;
    private readonly IIndividualTicketRepository _individualTicketRepository;

    public AddToCartHandler(
        IPurchaseOfferRepository purchaseOfferRepository,
        ICartRepository cartRepository,
        ISeasonTicketRepository seasonTicketRepository,
        IIndividualTicketRepository individualTicketRepository)
    {
        _purchaseOfferRepository = purchaseOfferRepository;
        _cartRepository = cartRepository;
        _seasonTicketRepository = seasonTicketRepository;
        _individualTicketRepository = individualTicketRepository;
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

            if (purchaseOffer.Status != "enabled")
            {
                return Task.FromResult(Result<AddToCartResponse>.Failure("This ticket is no longer available for purchase")
                    .WithCode((int)ResultCode.BadRequest));
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

            // 4. Izračunaj cenu na osnovu tipa karte
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
                // Cena za individualnu kartu se računa dinamički - za sada je 0
                price = 0;
            }

            // 5. Dodaj u korpu
            var cartItem = new CartItem
            {
                IdCart = currentCart.IdCart,
                IdPurchaseOffer = request.PurchaseOfferId,
                AddedAt = DateOnly.FromDateTime(DateTime.Now),
                Price = price
            };

            _cartRepository.AddItemToCart(cartItem);

            // 6. Označi kartu kao prodatu (ne može više da se kupuje)
            _purchaseOfferRepository.UpdateStatus(request.PurchaseOfferId, "disabled");

            // 7. Ako je sezonska karta kupljena, onemogući individualne karte za ta mesta u sezoni
            if (purchaseOffer.Type == "season ticket")
            {
                DisableIndividualTicketsForSeasonTicket(purchaseOffer);
            }

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
            // Log error but don't fail the main operation
            // In production, you would use proper logging here
            Console.WriteLine($"Error disabling individual tickets: {ex.Message}");
        }
    }
}
