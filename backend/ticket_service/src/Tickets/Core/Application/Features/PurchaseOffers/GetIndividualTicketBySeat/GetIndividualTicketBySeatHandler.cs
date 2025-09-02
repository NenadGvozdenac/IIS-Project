using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.GetIndividualTicketBySeat;

public class GetIndividualTicketBySeatHandler : IRequestHandler<GetIndividualTicketBySeatQuery, Result<GetIndividualTicketBySeatResponse>>
{
    private readonly ISeatRepository _seatRepository;
    private readonly IPurchaseOfferRepository _purchaseOfferRepository;
    private readonly IIndividualTicketRepository _individualTicketRepository;

    public GetIndividualTicketBySeatHandler(
        ISeatRepository seatRepository,
        IPurchaseOfferRepository purchaseOfferRepository,
        IIndividualTicketRepository individualTicketRepository)
    {
        _seatRepository = seatRepository;
        _purchaseOfferRepository = purchaseOfferRepository;
        _individualTicketRepository = individualTicketRepository;
    }

    public Task<Result<GetIndividualTicketBySeatResponse>> Handle(GetIndividualTicketBySeatQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Pronađi sedište po zoni, redu i broju
            var seat = _seatRepository.GetByZoneAndPosition(request.ZoneId, request.Row, request.Number);
            if (seat == null)
            {
                return Task.FromResult(Result<GetIndividualTicketBySeatResponse>.Failure($"Seat not found in zone {request.ZoneId}, row {request.Row}, number {request.Number}")
                    .WithCode((int)ResultCode.NotFound));
            }

            // 2. Pronađi individualnu kartu za to sedište i meč
            var individualTicketOffers = _purchaseOfferRepository.GetIndividualTicketsBySeat(seat.IdSeat)
                .Where(po => po.Type == "individual ticket" && po.Status == "enabled");

            // Filtriraj po meču
            var matchingOffer = individualTicketOffers.FirstOrDefault(po =>
            {
                var individualTicket = _individualTicketRepository.GetByPurchaseOfferId(po.IdPurchaseOffer);
                return individualTicket?.IdMatch == request.MatchId;
            });

            if (matchingOffer == null)
            {
                return Task.FromResult(Result<GetIndividualTicketBySeatResponse>.Failure($"No individual ticket found for this seat and match")
                    .WithCode((int)ResultCode.NotFound));
            }

            // 3. Dohvati dodatne informacije o individualnoj karti
            var individualTicket = _individualTicketRepository.GetByPurchaseOfferId(matchingOffer.IdPurchaseOffer);
            if (individualTicket == null)
            {
                return Task.FromResult(Result<GetIndividualTicketBySeatResponse>.Failure($"Individual ticket details not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var response = new GetIndividualTicketBySeatResponse
            {
                IdPurchaseOffer = matchingOffer.IdPurchaseOffer,
                Name = matchingOffer.Name,
                Description = matchingOffer.Description,
                Status = matchingOffer.Status,
                ReleasedAt = matchingOffer.ReleasedAt,
                CreatedAt = matchingOffer.CreatedAt,
                ExpiresAt = matchingOffer.ExpiresAt,
                IdSeat = matchingOffer.IdSeat,
                IdMatch = individualTicket.IdMatch,
                Price = 0, // Individualne karte imaju cenu 0 za sada
                SeatRow = seat.Row,
                SeatNumber = seat.Number,
                SeatType = seat.Type,
                SeatDirection = seat.Direction,
                SeatStatus = seat.Status,
                ZoneName = seat.IdZoneNavigation?.Name ?? "",
                ZoneRank = seat.IdZoneNavigation?.Rank ?? 0,
                MatchName = individualTicket.IdMatchNavigation.Name
            };

            return Task.FromResult(Result<GetIndividualTicketBySeatResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetIndividualTicketBySeatResponse>.Failure($"An error occurred while retrieving individual ticket: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
