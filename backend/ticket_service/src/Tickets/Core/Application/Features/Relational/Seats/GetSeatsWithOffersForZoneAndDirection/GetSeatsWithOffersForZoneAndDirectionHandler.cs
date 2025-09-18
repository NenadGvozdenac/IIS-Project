using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;
using ticket_service.src.Tickets.Core.Domain.Entities.Relational;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Seats.GetSeatsWithOffersForZoneAndDirection;

public class GetSeatsWithOffersForZoneAndDirectionHandler : IRequestHandler<GetSeatsWithOffersForZoneAndDirectionQuery, Result<List<GetSeatsWithOffersForZoneAndDirectionResponse>>>
{
    private readonly ISeatRepository _seatRepository;
    private readonly IPurchaseOfferRepository _purchaseOfferRepository;
    private readonly IIndividualTicketRepository _individualTicketRepository;
    private readonly ICartRepository _cartRepository;
    private readonly ITicketPriceCalculationService _priceCalculationService;

    public GetSeatsWithOffersForZoneAndDirectionHandler(
        ISeatRepository seatRepository,
        IPurchaseOfferRepository purchaseOfferRepository,
        IIndividualTicketRepository individualTicketRepository,
        ICartRepository cartRepository,
        ITicketPriceCalculationService priceCalculationService)
    {
        _seatRepository = seatRepository;
        _purchaseOfferRepository = purchaseOfferRepository;
        _individualTicketRepository = individualTicketRepository;
        _cartRepository = cartRepository;
        _priceCalculationService = priceCalculationService;
    }

    public Task<Result<List<GetSeatsWithOffersForZoneAndDirectionResponse>>> Handle(GetSeatsWithOffersForZoneAndDirectionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Get all seats for the specified zone and direction
            var seats = _seatRepository.GetByZoneId(request.ZoneId)
                .Where(seat => seat.Direction == request.Direction)
                .OrderBy(seat => seat.Row)
                .ThenBy(seat => seat.Number)
                .ToList();

            if (!seats.Any())
                return Task.FromResult(Result<List<GetSeatsWithOffersForZoneAndDirectionResponse>>.Success(new List<GetSeatsWithOffersForZoneAndDirectionResponse>()));

            var seatIds = seats.Select(s => s.IdSeat).ToList();

            // 2. BULK: Get all purchase offers for these seats in one query
            var allPurchaseOffers = _purchaseOfferRepository.GetAll()
                .Where(po => seatIds.Contains(po.IdSeat))
                .ToList();

            // 3. BULK: Get all individual tickets for this match in one query
            var matchIndividualTickets = _individualTicketRepository.GetByMatch(request.MatchId).ToList();
            var individualTicketOfferIds = matchIndividualTickets.Select(it => it.IdPurchaseOffer).ToHashSet();

            // 4. BULK: Get all cart items for season tickets in one query
            var seasonTicketOffers = allPurchaseOffers
                .Where(po => po.Type == "season ticket" && (po.Status == "bought" || po.Status == "enabled"))
                .ToList();

            var seasonTicketOfferIds = seasonTicketOffers.Select(sto => sto.IdPurchaseOffer).ToHashSet();

            var relevantCartItems = new List<CartItem>();
            var relevantCarts = new List<Cart>();

            if (seasonTicketOfferIds.Any())
            {
                relevantCarts = _cartRepository.GetAll()
                    .Where(c => c.Status == "bought" || c.Status == "active")
                    .ToList();

                relevantCartItems = relevantCarts
                    .SelectMany(c => c.CartItems)
                    .Where(ci => seasonTicketOfferIds.Contains(ci.IdPurchaseOffer))
                    .ToList();
            }

            // 5. Create lookup dictionaries for O(1) access
            var purchaseOffersBySeat = allPurchaseOffers
                .GroupBy(po => po.IdSeat)
                .ToDictionary(g => g.Key, g => g.ToList());

            var seasonTicketConflictsBySeat = CreateSeasonTicketConflictsLookup(
                seasonTicketOffers, relevantCartItems, relevantCarts, matchIndividualTickets.FirstOrDefault());

            // 6. Build responses in O(n) time
            var responses = new List<GetSeatsWithOffersForZoneAndDirectionResponse>();

            foreach (var seat in seats)
            {
                var response = new GetSeatsWithOffersForZoneAndDirectionResponse
                {
                    IdSeat = seat.IdSeat,
                    SeatRow = seat.Row,
                    SeatNumber = seat.Number,
                    SeatType = seat.Type,
                    SeatDirection = seat.Direction,
                    SeatStatus = seat.Status,
                    IdZone = seat.IdZone ?? 0,
                    ZoneName = seat.IdZoneNavigation?.Name,
                    HasOffer = false,
                    HasSeasonTicketConflict = false
                };

                // Check for season ticket conflicts (O(1) lookup)
                if (seasonTicketConflictsBySeat.TryGetValue(seat.IdSeat, out var conflict))
                {
                    response.HasSeasonTicketConflict = true;
                    response.ConflictReason = conflict.ConflictReason;
                    responses.Add(response);
                    continue;
                }

                // Check for individual ticket offers (O(1) lookup)
                if (seat.Status == "enabled" && purchaseOffersBySeat.TryGetValue(seat.IdSeat, out var offers))
                {
                    var individualOffer = offers.FirstOrDefault(po => 
                        po.Type == "individual ticket" && 
                        po.Status == "enabled" && 
                        individualTicketOfferIds.Contains(po.IdPurchaseOffer));

                    if (individualOffer != null)
                    {
                        response.HasOffer = true;
                        response.IdPurchaseOffer = individualOffer.IdPurchaseOffer;
                        response.OfferName = individualOffer.Name;
                        response.OfferDescription = individualOffer.Description;
                        response.OfferStatus = individualOffer.Status;
                        response.ReleasedAt = individualOffer.ReleasedAt;
                        response.ExpiresAt = individualOffer.ExpiresAt;

                        // Calculate dynamic price
                        if (seat.IdZone != null)
                        {
                            response.Price = _priceCalculationService.CalculateTicketPrice(request.MatchId, seat.IdZone.Value);
                        }

                        // Get match name
                        var matchTicket = matchIndividualTickets.FirstOrDefault(it => it.IdPurchaseOffer == individualOffer.IdPurchaseOffer);
                        response.MatchName = matchTicket?.IdMatchNavigation?.Name;
                    }
                }

                responses.Add(response);
            }

            return Task.FromResult(Result<List<GetSeatsWithOffersForZoneAndDirectionResponse>>.Success(responses));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<List<GetSeatsWithOffersForZoneAndDirectionResponse>>.Failure($"An error occurred while retrieving seats with offers: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }

    private Dictionary<int, SeasonTicketConflictResult> CreateSeasonTicketConflictsLookup(
        List<PurchaseOffer> seasonTicketOffers,
        List<CartItem> relevantCartItems,
        List<Cart> relevantCarts,
        IndividualTicket? matchTicket)
    {
        var conflicts = new Dictionary<int, SeasonTicketConflictResult>();

        if (matchTicket?.IdMatchNavigation == null)
            return conflicts;

        var matchDate = DateOnly.FromDateTime(matchTicket.IdMatchNavigation.ScheduledAt);
        
        // Create lookup for cart items by purchase offer
        var cartItemsByOffer = relevantCartItems
            .GroupBy(ci => ci.IdPurchaseOffer)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Create lookup for carts by ID
        var cartsById = relevantCarts.ToDictionary(c => c.IdCart);

        foreach (var seasonTicketOffer in seasonTicketOffers)
        {
            if (!cartItemsByOffer.TryGetValue(seasonTicketOffer.IdPurchaseOffer, out var cartItems))
                continue;

            foreach (var cartItem in cartItems)
            {
                // Check if season ticket is valid for this match date
                if (cartItem.ValidFrom == null || matchDate >= cartItem.ValidFrom)
                {
                    if (cartsById.TryGetValue(cartItem.IdCart, out var cart))
                    {
                        var conflictReason = cart.Status == "bought" 
                            ? "This seat is covered by an active season ticket"
                            : "This seat is reserved by a season ticket in someone's cart";

                        conflicts[seasonTicketOffer.IdSeat] = new SeasonTicketConflictResult
                        {
                            HasConflict = true,
                            ConflictReason = conflictReason
                        };
                        break; // Found conflict for this seat, no need to check more
                    }
                }
            }
        }

        return conflicts;
    }

    private class SeasonTicketConflictResult
    {
        public bool HasConflict { get; set; }
        public string? ConflictReason { get; set; }
    }
}