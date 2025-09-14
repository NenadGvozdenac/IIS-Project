using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;
using ticket_service.src.Tickets.Core.Domain.Entities;

namespace ticket_service.src.Tickets.Core.Application.Features.Matches.EnableTicketsForUpcomingMatches;

public class EnableTicketsForUpcomingMatchesHandler : IRequestHandler<EnableTicketsForUpcomingMatchesQuery, Result<EnableTicketsForUpcomingMatchesResponse>>
{
    private readonly IMatchRepository _matchRepository;
    private readonly ISeatRepository _seatRepository;
    private readonly IPurchaseOfferRepository _purchaseOfferRepository;
    private readonly IIndividualTicketRepository _individualTicketRepository;
    private readonly ITicketPriceParameterRepository _ticketPriceParameterRepository;
    private readonly IZoneRepository _zoneRepository;

    public EnableTicketsForUpcomingMatchesHandler(
        IMatchRepository matchRepository,
        ISeatRepository seatRepository,
        IPurchaseOfferRepository purchaseOfferRepository,
        IIndividualTicketRepository individualTicketRepository,
        ITicketPriceParameterRepository ticketPriceParameterRepository,
        IZoneRepository zoneRepository)
    {
        _matchRepository = matchRepository;
        _seatRepository = seatRepository;
        _purchaseOfferRepository = purchaseOfferRepository;
        _individualTicketRepository = individualTicketRepository;
        _ticketPriceParameterRepository = ticketPriceParameterRepository;
        _zoneRepository = zoneRepository;
    }

    public Task<Result<EnableTicketsForUpcomingMatchesResponse>> Handle(EnableTicketsForUpcomingMatchesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var match = _matchRepository.GetById(request.MatchId);

            if (match == null)
            {
                return Task.FromResult(Result<EnableTicketsForUpcomingMatchesResponse>.Failure($"Match with ID {request.MatchId} not found.")
                    .WithCode((int)ResultCode.NotFound));
            }

            // Check if the match is within 5 days
            var currentDate = DateTime.UtcNow;
            var daysDifference = (match.ScheduledAt - currentDate).TotalDays;

            if (daysDifference > 5)
            {
                return Task.FromResult(Result<EnableTicketsForUpcomingMatchesResponse>.Failure($"Match is scheduled more than 5 days away (in {daysDifference:F1} days). Tickets can only be enabled for matches within 5 days.")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (daysDifference < 0)
            {
                return Task.FromResult(Result<EnableTicketsForUpcomingMatchesResponse>.Failure("Cannot enable tickets for past matches.")
                    .WithCode((int)ResultCode.BadRequest));
            }

            // Check if ticket price parameters exist for all zones for this match
            var validationResult = ValidateTicketPriceParametersForAllZones(request.MatchId);
            if (!validationResult.IsSuccess)
            {
                return Task.FromResult(Result<EnableTicketsForUpcomingMatchesResponse>.Failure(validationResult.ErrorMessage)
                    .WithCode((int)ResultCode.BadRequest));
            }

            bool wasAlreadyEnabled = match.TicketsForSale;
            
            // Enable tickets for sale
            match.TicketsForSale = true;
            
            // Set the timestamp when tickets went on sale if not already set
            if (!wasAlreadyEnabled)
            {
                match.TicketsWentOnSale = DateTime.UtcNow;
            }

            _matchRepository.Update(match);

            // Create individual tickets for all enabled seats if not already created
            int createdTicketsCount = 0;
            if (!wasAlreadyEnabled)
            {
                createdTicketsCount = CreateIndividualTicketsForAllZones(match.IdMatch);
            }

            var response = new EnableTicketsForUpcomingMatchesResponse
            {
                IdMatch = match.IdMatch,
                Name = match.Name,
                ScheduledAt = match.ScheduledAt,
                Hall = match.Hall,
                WasAlreadyEnabled = wasAlreadyEnabled,
                TicketsEnabledAt = match.TicketsWentOnSale ?? DateTime.UtcNow,
                CreatedTicketsCount = createdTicketsCount,
                Message = wasAlreadyEnabled 
                    ? "Tickets were already enabled for this match."
                    : $"Tickets have been successfully enabled for this match. Created {createdTicketsCount} individual tickets for all zones."
            };

            return Task.FromResult(Result<EnableTicketsForUpcomingMatchesResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<EnableTicketsForUpcomingMatchesResponse>.Failure($"An error occurred while enabling tickets: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }

    private int CreateIndividualTicketsForAllZones(int matchId)
    {
        // Get the match details for naming and expiration
        var match = _matchRepository.GetById(matchId);
        if (match == null) return 0;

        // Get all enabled seats
        var enabledSeats = _seatRepository.GetEnabledSeats();
        int createdCount = 0;

        foreach (var seat in enabledSeats)
        {
            // Create purchase offer for individual ticket following the DDL pattern
            var purchaseOffer = new PurchaseOffer
            {
                Name = $"Individual Ticket - {match.Name} - {seat.IdZoneNavigation?.Name ?? "Zone"} Row {seat.Row} Seat {seat.Number} ({seat.Direction})",
                Description = $"Single match ticket for {match.Name} in {seat.IdZoneNavigation?.Name ?? "Zone"}, Row {seat.Row}, Seat {seat.Number} ({seat.Direction} side)",
                Type = "individual ticket",
                Status = "enabled",
                ReleasedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                ExpiresAt = DateOnly.FromDateTime(match.ScheduledAt), // Set expiration to match scheduled time
                IdSeat = seat.IdSeat
            };

            var createdPurchaseOffer = _purchaseOfferRepository.Create(purchaseOffer);

            // Create individual ticket linking the purchase offer to the match
            var individualTicket = new IndividualTicket
            {
                IdPurchaseOffer = createdPurchaseOffer.IdPurchaseOffer,
                IdMatch = matchId
            };

            _individualTicketRepository.Create(individualTicket);
            createdCount++;
        }

        return createdCount;
    }

    private (bool IsSuccess, string ErrorMessage) ValidateTicketPriceParametersForAllZones(int matchId)
    {
        // Get all zones that have enabled seats
        var enabledSeats = _seatRepository.GetEnabledSeats();
        var zonesWithEnabledSeats = enabledSeats
            .Where(s => s.IdZone.HasValue)
            .Select(s => s.IdZone!.Value)
            .Distinct()
            .ToList();

        if (!zonesWithEnabledSeats.Any())
        {
            return (false, "No enabled seats found in any zone.");
        }

        // Check if ticket price parameters exist for all zones for this match
        var missingZones = new List<int>();
        
        foreach (var zoneId in zonesWithEnabledSeats)
        {
            var parameter = _ticketPriceParameterRepository.GetByMatchZoneAndUser(matchId, zoneId);
            if (parameter == null)
            {
                missingZones.Add(zoneId);
            }
        }

        if (missingZones.Any())
        {
            var zoneNames = new List<string>();
            foreach (var zoneId in missingZones)
            {
                var zone = _zoneRepository.GetById(zoneId);
                zoneNames.Add(zone?.Name ?? $"Zone {zoneId}");
            }

            return (false, $"Ticket price parameters are missing for the following zones: {string.Join(", ", zoneNames)}. " +
                          "A club owner must create pricing parameters for all zones before tickets can be enabled.");
        }

        return (true, string.Empty);
    }
}