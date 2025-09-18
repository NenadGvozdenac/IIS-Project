using Microsoft.EntityFrameworkCore;
using ticket_service.src.Tickets.Core.Application.Features.Relational.Statistics.GetMatchStatistics;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

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
}