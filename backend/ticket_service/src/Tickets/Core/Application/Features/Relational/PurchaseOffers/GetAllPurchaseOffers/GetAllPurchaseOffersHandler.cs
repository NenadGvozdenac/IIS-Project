using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.PurchaseOffers.GetAllPurchaseOffers;

public class GetAllPurchaseOffersHandler : IRequestHandler<GetAllPurchaseOffersQuery, Result<IEnumerable<GetAllPurchaseOffersResponse>>>
{
    private readonly IPurchaseOfferRepository _purchaseOfferRepository;
    private readonly ISeasonTicketRepository _seasonTicketRepository;

    public GetAllPurchaseOffersHandler(IPurchaseOfferRepository purchaseOfferRepository, ISeasonTicketRepository seasonTicketRepository)
    {
        _purchaseOfferRepository = purchaseOfferRepository;
        _seasonTicketRepository = seasonTicketRepository;
    }

    public Task<Result<IEnumerable<GetAllPurchaseOffersResponse>>> Handle(GetAllPurchaseOffersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var purchaseOffers = _purchaseOfferRepository.GetAll();

            var response = purchaseOffers.Select(po =>
            {
                // Izračunaj cenu na osnovu tipa karte
                decimal price = 0;
                if (po.Type == "season ticket")
                {
                    var seasonTicket = _seasonTicketRepository.GetByPurchaseOfferId(po.IdPurchaseOffer);
                    if (seasonTicket != null)
                    {
                        price = seasonTicket.TicketPrice;
                    }
                }
                // Individual tickets imaju cenu 0 za sada

                return new GetAllPurchaseOffersResponse
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
                    SeatRow = po.IdSeatNavigation.Row,
                    SeatNumber = po.IdSeatNavigation.Number,
                    SeatType = po.IdSeatNavigation.Type,
                    SeatDirection = po.IdSeatNavigation.Direction,
                    SeatStatus = po.IdSeatNavigation.Status,
                    ZoneName = po.IdSeatNavigation.IdZoneNavigation?.Name,
                    ZoneRank = po.IdSeatNavigation.IdZoneNavigation?.Rank
                };
            });

            return Task.FromResult(Result<IEnumerable<GetAllPurchaseOffersResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<IEnumerable<GetAllPurchaseOffersResponse>>.Failure($"An error occurred while retrieving purchase offers: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
