using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.GetSeasonTickets;

public class GetSeasonTicketsHandler : IRequestHandler<GetSeasonTicketsQuery, Result<IEnumerable<GetSeasonTicketsResponse>>>
{
    private readonly IPurchaseOfferRepository _purchaseOfferRepository;
    private readonly ISeasonTicketRepository _seasonTicketRepository;

    public GetSeasonTicketsHandler(IPurchaseOfferRepository purchaseOfferRepository, ISeasonTicketRepository seasonTicketRepository)
    {
        _purchaseOfferRepository = purchaseOfferRepository;
        _seasonTicketRepository = seasonTicketRepository;
    }

    public Task<Result<IEnumerable<GetSeasonTicketsResponse>>> Handle(GetSeasonTicketsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var seasonTickets = _purchaseOfferRepository.GetEnabledByType("season ticket");

            var response = seasonTickets.Select(po =>
            {
                // Pronađi cenu sezonske karte
                decimal price = 0;
                var seasonTicket = _seasonTicketRepository.GetByPurchaseOfferId(po.IdPurchaseOffer);
                if (seasonTicket != null)
                {
                    price = seasonTicket.TicketPrice;
                }

                return new GetSeasonTicketsResponse
                {
                    IdPurchaseOffer = po.IdPurchaseOffer,
                    Name = po.Name,
                    Description = po.Description,
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

            return Task.FromResult(Result<IEnumerable<GetSeasonTicketsResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<IEnumerable<GetSeasonTicketsResponse>>.Failure($"An error occurred while retrieving season tickets: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
