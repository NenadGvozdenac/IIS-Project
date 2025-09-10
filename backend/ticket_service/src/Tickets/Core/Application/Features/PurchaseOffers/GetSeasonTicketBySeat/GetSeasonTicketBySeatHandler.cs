using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.GetSeasonTicketBySeat;

public class GetSeasonTicketBySeatHandler : IRequestHandler<GetSeasonTicketBySeatQuery, Result<GetSeasonTicketBySeatResponse>>
{
    private readonly ISeatRepository _seatRepository;
    private readonly IPurchaseOfferRepository _purchaseOfferRepository;
    private readonly ISeasonTicketRepository _seasonTicketRepository;

    public GetSeasonTicketBySeatHandler(
        ISeatRepository seatRepository,
        IPurchaseOfferRepository purchaseOfferRepository,
        ISeasonTicketRepository seasonTicketRepository)
    {
        _seatRepository = seatRepository;
        _purchaseOfferRepository = purchaseOfferRepository;
        _seasonTicketRepository = seasonTicketRepository;
    }

    public Task<Result<GetSeasonTicketBySeatResponse>> Handle(GetSeasonTicketBySeatQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Pronađi sedište po zoni, redu, broju i smeru
            var seat = _seatRepository.GetByZoneAndPositionAndDirection(request.ZoneId, request.Row, request.Number, request.Direction);
            if (seat == null)
            {
                return Task.FromResult(Result<GetSeasonTicketBySeatResponse>.Failure($"Seat not found in zone {request.ZoneId}, row {request.Row}, number {request.Number}, direction {request.Direction}")
                    .WithCode((int)ResultCode.NotFound));
            }

            // 2. Pronađi sezonsku kartu za to sedište
            var seasonTicketOffers = _purchaseOfferRepository.GetSeasonTicketsBySeat(seat.IdSeat, request.SeasonId);
            var seasonTicketOffer = seasonTicketOffers.FirstOrDefault();

            if (seasonTicketOffer == null)
            {
                return Task.FromResult(Result<GetSeasonTicketBySeatResponse>.Failure($"No season ticket found for this seat")
                    .WithCode((int)ResultCode.NotFound));
            }

            // 3. Dohvati dodatne informacije o sezonskoj karti
            var seasonTicket = _seasonTicketRepository.GetByPurchaseOfferId(seasonTicketOffer.IdPurchaseOffer);
            if (seasonTicket == null)
            {
                return Task.FromResult(Result<GetSeasonTicketBySeatResponse>.Failure($"Season ticket details not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var response = new GetSeasonTicketBySeatResponse
            {
                IdPurchaseOffer = seasonTicketOffer.IdPurchaseOffer,
                Name = seasonTicketOffer.Name,
                Description = seasonTicketOffer.Description,
                Status = seasonTicketOffer.Status,
                ReleasedAt = seasonTicketOffer.ReleasedAt,
                CreatedAt = seasonTicketOffer.CreatedAt,
                ExpiresAt = seasonTicketOffer.ExpiresAt,
                IdSeat = seasonTicketOffer.IdSeat,
                Price = seasonTicket.TicketPrice,
                SeatRow = seat.Row,
                SeatNumber = seat.Number,
                SeatType = seat.Type,
                SeatDirection = seat.Direction,
                SeatStatus = seat.Status,
                ZoneName = seat.IdZoneNavigation?.Name ?? "",
                ZoneRank = seat.IdZoneNavigation?.Rank ?? 0,
                SeasonName = seasonTicket.IdSeasonNavigation.Name
            };

            return Task.FromResult(Result<GetSeasonTicketBySeatResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetSeasonTicketBySeatResponse>.Failure($"An error occurred while retrieving season ticket: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
