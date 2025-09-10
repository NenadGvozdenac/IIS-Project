using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.CheckSeasonTicketConflict;

public class CheckSeasonTicketConflictHandler : IRequestHandler<CheckSeasonTicketConflictQuery, Result<CheckSeasonTicketConflictResponse>>
{
    private readonly IPurchaseOfferRepository _purchaseOfferRepository;
    private readonly ISeasonTicketRepository _seasonTicketRepository;
    private readonly ICartRepository _cartRepository;

    public CheckSeasonTicketConflictHandler(
        IPurchaseOfferRepository purchaseOfferRepository,
        ISeasonTicketRepository seasonTicketRepository,
        ICartRepository cartRepository)
    {
        _purchaseOfferRepository = purchaseOfferRepository;
        _seasonTicketRepository = seasonTicketRepository;
        _cartRepository = cartRepository;
    }

    public Task<Result<CheckSeasonTicketConflictResponse>> Handle(CheckSeasonTicketConflictQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var response = new CheckSeasonTicketConflictResponse
            {
                HasConflict = false
            };

            if (!DateTime.TryParse(request.MatchDate, out var matchDate))
            {
                return Task.FromResult(Result<CheckSeasonTicketConflictResponse>.Failure("Invalid match date format")
                    .WithCode((int)ResultCode.BadRequest));
            }

            // Find all season tickets for this seat that are bought OR in active carts
            var seasonTickets = _purchaseOfferRepository.GetAll()
                .Where(po => po.IdSeat == request.SeatId && 
                           po.Type == "season ticket" && 
                           (po.Status == "bought" || po.Status == "enabled"));

            foreach (var seasonTicketOffer in seasonTickets)
            {
                // Check if this season ticket is valid for the match date
                var cartItems = _cartRepository.GetAll()
                    .Where(c => c.Status == "bought" || c.Status == "active") // Check both bought and active carts
                    .SelectMany(c => c.CartItems)
                    .Where(ci => ci.IdPurchaseOffer == seasonTicketOffer.IdPurchaseOffer);

                foreach (var cartItem in cartItems)
                {
                    // If season ticket is valid (valid_from is null or <= match date)
                    if (cartItem.ValidFrom == null || DateOnly.FromDateTime(matchDate) >= cartItem.ValidFrom)
                    {
                        var seasonTicket = _seasonTicketRepository.GetByPurchaseOfferId(seasonTicketOffer.IdPurchaseOffer);
                        
                        response.HasConflict = true;
                        
                        // Determine conflict reason based on cart status
                        var cart = _cartRepository.GetAll()
                            .FirstOrDefault(c => c.CartItems.Any(ci => ci.IdPurchaseOffer == seasonTicketOffer.IdPurchaseOffer));
                        
                        if (cart?.Status == "bought")
                        {
                            response.ConflictReason = "This seat is covered by an active season ticket";
                        }
                        else if (cart?.Status == "active")
                        {
                            response.ConflictReason = "This seat is reserved by a season ticket in someone's cart";
                        }
                        else
                        {
                            response.ConflictReason = "This seat is not available";
                        }
                        
                        response.SeasonTicketValidFrom = cartItem.ValidFrom?.ToDateTime(TimeOnly.MinValue);
                        
                        if (cart?.IdUserNavigation != null)
                        {
                            response.SeasonTicketHolder = cart.IdUserNavigation.Name ?? "Season ticket holder";
                        }
                        
                        break;
                    }
                }
                
                if (response.HasConflict) break;
            }

            return Task.FromResult(Result<CheckSeasonTicketConflictResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CheckSeasonTicketConflictResponse>.Failure($"An error occurred while checking season ticket conflicts: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
