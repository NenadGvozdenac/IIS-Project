using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.PurchaseOffers.GetExistingSeasonTickets;

public class GetExistingSeasonTicketsHandler : IRequestHandler<GetExistingSeasonTicketsQuery, Result<IEnumerable<GetExistingSeasonTicketsResponse>>>
{
    private readonly IPurchaseOfferRepository _purchaseOfferRepository;

    public GetExistingSeasonTicketsHandler(IPurchaseOfferRepository purchaseOfferRepository)
    {
        _purchaseOfferRepository = purchaseOfferRepository;
    }

    public Task<Result<IEnumerable<GetExistingSeasonTicketsResponse>>> Handle(GetExistingSeasonTicketsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var existingSeasonTickets = _purchaseOfferRepository.GetExistingSeasonTicketsByZoneAndSeason(request.ZoneId, request.SeasonId);

            var response = existingSeasonTickets.Select(po =>
            {
                // Get user information from the cart that purchased this ticket
                var cartItem = po.CartItems.FirstOrDefault();
                var userId = cartItem?.IdCartNavigation?.IdUser ?? 0;
                var userName = "Unknown"; // You might want to include user name if needed

                return new GetExistingSeasonTicketsResponse
                {
                    IdPurchaseOffer = po.IdPurchaseOffer,
                    IdSeat = po.IdSeat,
                    SeatRow = po.IdSeatNavigation.Row,
                    SeatNumber = po.IdSeatNavigation.Number,
                    SeatDirection = po.IdSeatNavigation.Direction,
                    UserId = userId,
                    UserName = userName,
                    Price = 0, // You might want to get the actual price from SeasonTicket
                    CreatedAt = po.CreatedAt
                };
            });

            return Task.FromResult(Result<IEnumerable<GetExistingSeasonTicketsResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<IEnumerable<GetExistingSeasonTicketsResponse>>.Failure($"An error occurred while retrieving existing season tickets: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
