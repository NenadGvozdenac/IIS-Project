using Microsoft.EntityFrameworkCore;
using ticket_service.src.Tickets.Core.Application.Features.Relational.Statistics.GetMatchStatistics;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;
using ticket_service.src.Tickets.Core.Domain.Entities.Relational;

namespace ticket_service.src.Tickets.Core.Infrastructure.Repositories.Relational;

public class StatisticsRepository : IStatisticsRepository
{
    private readonly TicketDbContext _ticketDbContext;

    public StatisticsRepository(TicketDbContext context)
    {
        _ticketDbContext = context;
    }

    public GetMatchStatisticsResponse? GetMatchStatistics(int matchId)
    {
        var match = _ticketDbContext.Matches
            .Include(m => m.IdCompetitionNavigation)
            .Include(m => m.IdSeasonNavigation)
            .Include(m => m.IdTeamNavigation)
            .FirstOrDefault(m => m.IdMatch == matchId);

        if (match == null)
            return null;

        // Get sales summaries for this match using scaffoldovani entity
        var salesSummaries = _ticketDbContext.MatchZoneSalesSummaries
            .Include(mzss => mzss.IdZoneNavigation)
            .Include(mzss => mzss.IdTicketPriceParameterNavigation)
            .Where(mzss => mzss.IdMatch == matchId)
            .ToList();

        // Get all zones and their total seats count
        var allZones = _ticketDbContext.Zones
            .Select(z => new
            {
                z.IdZone,
                z.Name,
                z.Rank,
                TotalSeats = _ticketDbContext.Seats.Count(s => s.IdZone == z.IdZone)
            })
            .OrderBy(z => z.Rank)
            .ToList();

        // Create zone statistics
        var zones = allZones.Select(zone =>
        {
            var summary = salesSummaries.FirstOrDefault(s => s.IdZone == zone.IdZone);
            var priceParam = summary?.IdTicketPriceParameterNavigation;

            return new ZoneStatistics
            {
                ZoneId = zone.IdZone,
                ZoneName = zone.Name,
                PriceParameter = priceParam?.PriceFactor,
                TimeParameter = priceParam?.TimeFactor,
                MinimumSeatPrice = priceParam?.MinimumSeatPrice,
                MaximumSeatPrice = priceParam?.MaximumSeatPrice,
                TotalTicketsSold = summary?.TotalTicketsSold ?? 0,
                TotalSeats = zone.TotalSeats,
                TotalRevenue = summary?.TotalRevenue ?? 0
            };
        }).ToList();

        var totalRevenue = zones.Sum(z => z.TotalRevenue);

        return new GetMatchStatisticsResponse
        {
            MatchId = match.IdMatch,
            MatchName = match.Name,
            ScheduledAt = match.ScheduledAt,
            Type = match.Type,
            City = match.City,
            Hall = match.Hall,
            IsInOurHall = match.IsInOurHall,
            TicketsWentOnSale = match.TicketsWentOnSale,
            CompetitionName = match.IdCompetitionNavigation?.Name ?? "",
            SeasonName = match.IdSeasonNavigation?.Name ?? "",
            TeamName = match.IdTeamNavigation?.Name ?? "",
            Zones = zones,
            TotalRevenue = totalRevenue
        };
    }

    public async Task<MatchSummaryReportResponse> GetMatchSummaryReportAsync()
    {
        var viewResults = await _ticketDbContext.MatchSummaryReportViews.ToListAsync();

        var results = viewResults.Select(view => new MatchSummaryReport
        {
            MatchId = view.MatchId ?? 0,
            MatchName = view.MatchName ?? string.Empty,
            MatchDate = view.MatchDate ?? DateTime.MinValue,
            MatchType = view.MatchType ?? string.Empty,
            City = view.City ?? string.Empty,
            Hall = view.Hall ?? string.Empty,
            SeasonName = view.SeasonName ?? string.Empty,
            CompetitionName = view.CompetitionName,
            TeamName = view.TeamName ?? string.Empty,
            TotalTicketsSold = view.TotalTicketsSold ?? 0,
            TotalRevenue = view.TotalRevenue ?? 0m,
            VipZoneTickets = view.VipZoneTickets ?? 0,
            VipZoneRevenue = view.VipZoneRevenue ?? 0m,
            RegularZoneTickets = view.RegularZoneTickets ?? 0,
            RegularZoneRevenue = view.RegularZoneRevenue ?? 0m,
            AverageTicketPrice = view.AverageTicketPrice ?? 0m,
            StadiumFillPercentage = view.StadiumFillPercentage ?? 0m,
            VipZoneFillPercentage = view.VipZoneFillPercentage ?? 0m,
            RegularZoneFillPercentage = view.RegularZoneFillPercentage ?? 0m,
            HighestSellingZone = view.HighestSellingZone ?? string.Empty,
            LowestSellingZone = view.LowestSellingZone ?? string.Empty,
            TrackingStatus = view.TrackingStatus ?? string.Empty,
            OurPoints = view.OurPoints ?? 0,
            OpponentPoints = view.OpponentPoints ?? 0
        }).ToList();

        return new MatchSummaryReportResponse
        {
            Matches = results,
            GeneratedAt = DateTime.UtcNow
        };
    }
}