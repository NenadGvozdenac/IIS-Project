using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;
using ticket_service.src.Tickets.Core.Domain.Entities;

namespace ticket_service.src.Tickets.Core.Application.Features.Seasons.EnableSeasonCards;

public class EnableSeasonTicketsHandler : IRequestHandler<EnableSeasonTicketsCommand, Result<EnableSeasonTicketsResponse>>
{
    private readonly ISeasonRepository _seasonRepository;
    private readonly ISeatRepository _seatRepository;
    private readonly IPurchaseOfferRepository _purchaseOfferRepository;
    private readonly ISeasonTicketRepository _seasonTicketRepository;

    public EnableSeasonTicketsHandler(
        ISeasonRepository seasonRepository,
        ISeatRepository seatRepository,
        IPurchaseOfferRepository purchaseOfferRepository,
        ISeasonTicketRepository seasonTicketRepository)
    {
        _seasonRepository = seasonRepository;
        _seatRepository = seatRepository;
        _purchaseOfferRepository = purchaseOfferRepository;
        _seasonTicketRepository = seasonTicketRepository;
    }

    public Task<Result<EnableSeasonTicketsResponse>> Handle(EnableSeasonTicketsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get the season by ID
            var season = _seasonRepository.GetById(request.IdSeason);
            if (season == null)
            {
                return Task.FromResult(Result<EnableSeasonTicketsResponse>.Failure("Season not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            // Check if tickets are already for sale
            if (season.TicketsForSale)
            {
                return Task.FromResult(Result<EnableSeasonTicketsResponse>.Failure("Season tickets are already for sale")
                    .WithCode((int)ResultCode.BadRequest));
            }

            // Validate zone prices
            if (request.ZonePrices == null || !request.ZonePrices.Any())
            {
                return Task.FromResult(Result<EnableSeasonTicketsResponse>.Failure("Zone prices must be provided")
                    .WithCode((int)ResultCode.BadRequest));
            }

            // Validate that all zones have positive prices
            var invalidZones = request.ZonePrices.Where(zp => zp.Price <= 0).ToList();
            if (invalidZones.Any())
            {
                return Task.FromResult(Result<EnableSeasonTicketsResponse>.Failure("All zone prices must be greater than 0")
                    .WithCode((int)ResultCode.BadRequest));
            }

            // Release season cards
            var result = _seasonRepository.ReleaseSeasonCards(request.IdSeason);
            if (!result)
            {
                return Task.FromResult(Result<EnableSeasonTicketsResponse>.Failure("Failed to enable season tickets")
                    .WithCode((int)ResultCode.InternalServerError));
            }

            // Create season tickets for all enabled seats with zone-specific pricing
            int createdTicketsCount = CreateSeasonTicketsForAllSeats(request.IdSeason, request.ZonePrices);

            // Get updated season
            var updatedSeason = _seasonRepository.GetById(request.IdSeason);

            var response = new EnableSeasonTicketsResponse
            {
                IdSeason = updatedSeason!.IdSeason,
                Name = updatedSeason.Name,
                TicketsForSale = updatedSeason.TicketsForSale,
                TicketsWentOnSale = updatedSeason.TicketsWentOnSale,
                CreatedTicketsCount = createdTicketsCount,
                Message = $"Season cards have been successfully released for sale. Created {createdTicketsCount} season tickets for all enabled seats."
            };

            return Task.FromResult(Result<EnableSeasonTicketsResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<EnableSeasonTicketsResponse>.Failure($"An error occurred while enabling season tickets: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }

    private int CreateSeasonTicketsForAllSeats(int seasonId, List<ZonePricing> zonePrices)
    {
        // Get the season details for naming and expiration
        var season = _seasonRepository.GetById(seasonId);
        if (season == null) return 0;

        // Get all enabled seats
        var enabledSeats = _seatRepository.GetEnabledSeats();
        int createdCount = 0;

        // Create a dictionary for quick price lookup by zone
        var zonePriceDict = zonePrices.ToDictionary(zp => zp.ZoneId, zp => zp.Price);

        foreach (var seat in enabledSeats)
        {
            // Skip seats that don't have a zone or don't have pricing defined
            if (!seat.IdZone.HasValue || !zonePriceDict.ContainsKey(seat.IdZone.Value))
            {
                continue; // Skip this seat if no pricing is defined for its zone
            }

            int ticketPrice = zonePriceDict[seat.IdZone.Value];

            // Create purchase offer for season ticket following the DDL pattern
            var purchaseOffer = new PurchaseOffer
            {
                Name = $"Season Ticket - {season.Name} - {seat.IdZoneNavigation?.Name ?? "Zone"} Row {seat.Row} Seat {seat.Number} ({seat.Direction})",
                Description = $"Full season ticket for {season.Name} in {seat.IdZoneNavigation?.Name ?? "Zone"}, Row {seat.Row}, Seat {seat.Number} ({seat.Direction} side)",
                Type = "season ticket",
                Status = "enabled",
                ReleasedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                ExpiresAt = season.EndedAt, // Set expiration to season end date
                IdSeat = seat.IdSeat
            };

            var createdPurchaseOffer = _purchaseOfferRepository.Create(purchaseOffer);

            // Create season ticket linking the purchase offer to the season with zone-specific price
            var seasonTicket = new SeasonTicket
            {
                IdPurchaseOffer = createdPurchaseOffer.IdPurchaseOffer,
                IdSeason = seasonId,
                TicketPrice = ticketPrice
            };

            _seasonTicketRepository.Create(seasonTicket);
            createdCount++;
        }

        return createdCount;
    }
}