using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.GetUserPurchaseHistory;

public class GetUserPurchaseHistoryHandler : IRequestHandler<GetUserPurchaseHistoryQuery, Result<IEnumerable<GetUserPurchaseHistoryResponse>>>
{
    private readonly IPurchaseOfferRepository _purchaseOfferRepository;
    private readonly ISeasonTicketRepository _seasonTicketRepository;
    private readonly IIndividualTicketRepository _individualTicketRepository;

    public GetUserPurchaseHistoryHandler(
        IPurchaseOfferRepository purchaseOfferRepository,
        ISeasonTicketRepository seasonTicketRepository,
        IIndividualTicketRepository individualTicketRepository)
    {
        _purchaseOfferRepository = purchaseOfferRepository;
        _seasonTicketRepository = seasonTicketRepository;
        _individualTicketRepository = individualTicketRepository;
    }

    public Task<Result<IEnumerable<GetUserPurchaseHistoryResponse>>> Handle(GetUserPurchaseHistoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var purchaseHistory = _purchaseOfferRepository.GetPurchaseHistoryByUserId(request.UserId);

            var response = purchaseHistory.Select(po =>
            {
                // Izračunaj cenu na osnovu tipa karte
                decimal price = 0;
                string? seasonName = null;
                string? matchName = null;
                DateOnly purchaseDate = DateOnly.FromDateTime(DateTime.Now); // Default, trebalo bi da dohvatiš iz cart_item

                if (po.Type == "season ticket")
                {
                    var seasonTicket = _seasonTicketRepository.GetByPurchaseOfferId(po.IdPurchaseOffer);
                    if (seasonTicket != null)
                    {
                        price = seasonTicket.TicketPrice;
                        seasonName = seasonTicket.IdSeasonNavigation.Name;
                    }
                }
                else if (po.Type == "individual ticket")
                {
                    var individualTicket = _individualTicketRepository.GetByPurchaseOfferId(po.IdPurchaseOffer);
                    if (individualTicket != null)
                    {
                        matchName = individualTicket.IdMatchNavigation.Name;
                    }
                    price = 0; // Individual tickets imaju cenu 0 za sada
                }

                // Dohvati purchase date iz cart items
                var cartItem = po.CartItems.FirstOrDefault();
                if (cartItem != null)
                {
                    purchaseDate = cartItem.AddedAt;
                    price = cartItem.Price; // Koristi cenu iz cart item-a
                }

                return new GetUserPurchaseHistoryResponse
                {
                    IdPurchaseOffer = po.IdPurchaseOffer,
                    Name = po.Name,
                    Description = po.Description,
                    Type = po.Type,
                    Status = po.Status,
                    ReleasedAt = po.ReleasedAt,
                    CreatedAt = po.CreatedAt,
                    ExpiresAt = po.ExpiresAt,
                    IdSeat = po.IdSeat,
                    Price = price,
                    PurchaseDate = purchaseDate,
                    SeatRow = po.IdSeatNavigation.Row,
                    SeatNumber = po.IdSeatNavigation.Number,
                    SeatType = po.IdSeatNavigation.Type,
                    SeatDirection = po.IdSeatNavigation.Direction,
                    SeatStatus = po.IdSeatNavigation.Status,
                    ZoneName = po.IdSeatNavigation.IdZoneNavigation?.Name,
                    ZoneRank = po.IdSeatNavigation.IdZoneNavigation?.Rank,
                    SeasonName = seasonName,
                    MatchName = matchName
                };
            });

            return Task.FromResult(Result<IEnumerable<GetUserPurchaseHistoryResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<IEnumerable<GetUserPurchaseHistoryResponse>>.Failure($"An error occurred while retrieving purchase history: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
