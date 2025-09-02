using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.GetIndividualTickets;

public class GetIndividualTicketsHandler : IRequestHandler<GetIndividualTicketsQuery, Result<IEnumerable<GetIndividualTicketsResponse>>>
{
    private readonly IIndividualTicketRepository _individualTicketRepository;
    private readonly IPurchaseOfferRepository _purchaseOfferRepository;

    public GetIndividualTicketsHandler(IIndividualTicketRepository individualTicketRepository, IPurchaseOfferRepository purchaseOfferRepository)
    {
        _individualTicketRepository = individualTicketRepository;
        _purchaseOfferRepository = purchaseOfferRepository;
    }

    public Task<Result<IEnumerable<GetIndividualTicketsResponse>>> Handle(GetIndividualTicketsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var individualTickets = _individualTicketRepository.GetAvailableByMatch(request.MatchId);

            var response = individualTickets.Select(it =>
            {
                var purchaseOffer = _purchaseOfferRepository.GetById(it.IdPurchaseOffer);
                return new GetIndividualTicketsResponse
                {
                    IdPurchaseOffer = it.IdPurchaseOffer,
                    Name = purchaseOffer!.Name,
                    Description = purchaseOffer.Description,
                    Status = purchaseOffer.Status,
                    ReleasedAt = purchaseOffer.ReleasedAt,
                    CreatedAt = purchaseOffer.CreatedAt,
                    ExpiresAt = purchaseOffer.ExpiresAt,
                    IdSeat = purchaseOffer.IdSeat,
                    IdMatch = it.IdMatch,
                    Price = 0, // Individualne karte imaju cenu 0 za sada
                    SeatRow = purchaseOffer.IdSeatNavigation.Row,
                    SeatNumber = purchaseOffer.IdSeatNavigation.Number,
                    SeatType = purchaseOffer.IdSeatNavigation.Type,
                    SeatDirection = purchaseOffer.IdSeatNavigation.Direction,
                    SeatStatus = purchaseOffer.IdSeatNavigation.Status,
                    ZoneName = purchaseOffer.IdSeatNavigation.IdZoneNavigation?.Name,
                    ZoneRank = purchaseOffer.IdSeatNavigation.IdZoneNavigation?.Rank,
                    MatchName = it.IdMatchNavigation.Name
                };
            });

            return Task.FromResult(Result<IEnumerable<GetIndividualTicketsResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<IEnumerable<GetIndividualTicketsResponse>>.Failure($"An error occurred while retrieving individual tickets: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
