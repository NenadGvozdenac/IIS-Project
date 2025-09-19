using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;
using ticket_service.src.Tickets.Core.Infrastructure.Database;
using Neo4j.Driver;

namespace ticket_service.src.Tickets.Core.Infrastructure.Repositories.Graph;

public class ReportsRepository : IGraphReportsRepository
{
    private readonly IGraphDatabaseContext _graphDbContext;

    public ReportsRepository(IGraphDatabaseContext graphDbContext)
    {
        _graphDbContext = graphDbContext;
    }

    public async Task<List<MatchTicketSalesReportDto>> GetMatchTicketSalesReport()
    {
        var query = @"
            MATCH (m:Match)
            OPTIONAL MATCH (m)<-[:IS_FOR_MATCH]-(t:IndividualTicket)<-[r:BOUGHT]-(:Customer)
            WITH m, count(r) AS tickets_sold, sum(r.price) AS total_revenue
            RETURN m.name AS match_name, 
                   m.scheduled_at AS scheduled_at,
                   m.hall AS hall,
                   tickets_sold, 
                   CASE WHEN total_revenue IS NULL THEN 0 ELSE total_revenue END AS total_revenue
            ORDER BY m.scheduled_at";

        var result = await _graphDbContext.RunAsync(query, new { });
        var reports = new List<MatchTicketSalesReportDto>();

        await foreach (var record in result)
        {
            reports.Add(new MatchTicketSalesReportDto
            {
                MatchName = record["match_name"].As<string>(),
                ScheduledAt = record["scheduled_at"].As<ZonedDateTime>().ToDateTimeOffset().DateTime,
                Hall = record["hall"].As<string>(),
                TotalTicketsSold = record["tickets_sold"].As<int>(),
                TotalRevenue = record["total_revenue"].As<decimal>()
            });
        }

        return reports;
    }

    public async Task<List<CustomerSpendingReportDto>> GetCustomerSpendingReport()
    {
        var query = @"
            MATCH (c:Customer)
            OPTIONAL MATCH (c)-[r:BOUGHT]->(t:IndividualTicket)-[:IS_FOR_MATCH]->(m:Match)
            WITH c, count(r) AS tickets_purchased, 
                 sum(r.price) AS total_spent,
                 count(DISTINCT m) AS matches_attended
            RETURN c.name + ' ' + c.surname AS customer_name,
                   c.email AS email,
                   tickets_purchased,
                   CASE WHEN total_spent IS NULL THEN 0 ELSE total_spent END AS total_spent,
                   matches_attended
            ORDER BY total_spent DESC";

        var result = await _graphDbContext.RunAsync(query, new { });
        var reports = new List<CustomerSpendingReportDto>();

        await foreach (var record in result)
        {
            reports.Add(new CustomerSpendingReportDto
            {
                CustomerName = record["customer_name"].As<string>(),
                Email = record["email"].As<string>(),
                TotalTicketsPurchased = record["tickets_purchased"].As<int>(),
                TotalAmountSpent = record["total_spent"].As<decimal>(),
                MatchesAttended = record["matches_attended"].As<int>()
            });
        }

        return reports;
    }

    public async Task<SectorAnalysisReportDto> GetSectorAnalysisReport()
    {
        // Prvo dobijamo osnovne podatke o sektorima
        var sectorQuery = @"
            MATCH (s:Seat)
            WITH DISTINCT s.direction AS sector
            OPTIONAL MATCH (s2:Seat {direction: sector})<-[:IS_FOR_SEAT]-(t:IndividualTicket)<-[r:BOUGHT]-(:Customer)
            OPTIONAL MATCH (t)-[:IS_FOR_MATCH]->(m:Match)
            WITH sector, 
                 count(r) AS total_tickets_sold,
                 sum(r.price) AS total_revenue,
                 avg(r.price) AS average_price
            RETURN sector, 
                   total_tickets_sold,
                   CASE WHEN total_revenue IS NULL THEN 0 ELSE total_revenue END AS total_revenue,
                   CASE WHEN average_price IS NULL THEN 0 ELSE average_price END AS average_price
            ORDER BY total_tickets_sold DESC";

        var result = await _graphDbContext.RunAsync(sectorQuery, new { });
        var sectorDetails = new List<SectorDetailDto>();
        var totalTicketsAll = 0;
        var totalRevenueAll = 0m;

        // Prvo čitamo sve sektore u memoriju
        var sectors = new List<(string sector, int totalTicketsSold, decimal totalRevenue, decimal averagePrice)>();
        
        await foreach (var record in result)
        {
            var sector = record["sector"].As<string>();
            var totalTicketsSold = record["total_tickets_sold"].As<int>();
            var totalRevenue = record["total_revenue"].As<decimal>();
            var averagePrice = record["average_price"].As<decimal>();

            totalTicketsAll += totalTicketsSold;
            totalRevenueAll += totalRevenue;

            sectors.Add((sector, totalTicketsSold, totalRevenue, averagePrice));
        }

        // Sada async pozivamo analizu za svaki sektor
        foreach (var (sector, totalTicketsSold, totalRevenue, averagePrice) in sectors)
        {
            var matchAnalysis = await GetSectorMatchAnalysis(sector);

            sectorDetails.Add(new SectorDetailDto
            {
                SectorName = sector,
                MatchAnalysis = matchAnalysis,
                AveragePrice = averagePrice,
                TotalTicketsSold = totalTicketsSold,
                TotalRevenue = totalRevenue,
                PopularityRank = "" // Popuniće se nakon sortiranja
            });
        }

        // Rangiranje sektora
        for (int i = 0; i < sectorDetails.Count; i++)
        {
            sectorDetails[i].PopularityRank = $"{i + 1}. place";
        }

        // Kreiranje summary-ja
        var summary = new SectorSummaryDto
        {
            MostPopularSector = sectorDetails.FirstOrDefault()?.SectorName ?? "N/A",
            HighestRevenueSector = sectorDetails.OrderByDescending(s => s.TotalRevenue).FirstOrDefault()?.SectorName ?? "N/A",
            HighestAveragePriceSector = sectorDetails.OrderByDescending(s => s.AveragePrice).FirstOrDefault()?.SectorName ?? "N/A",
            TotalTicketsSoldAllSectors = totalTicketsAll,
            TotalRevenueAllSectors = totalRevenueAll
        };

        return new SectorAnalysisReportDto
        {
            ReportGeneratedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            SectorDetails = sectorDetails,
            Summary = summary
        };
    }

    private async Task<List<SectorMatchAnalysisDto>> GetSectorMatchAnalysis(string sector)
    {
        var query = @"
            MATCH (m:Match)
            OPTIONAL MATCH (m)<-[:IS_FOR_MATCH]-(t:IndividualTicket)-[:IS_FOR_SEAT]->(s:Seat {direction: $sector})
            OPTIONAL MATCH (t)<-[r:BOUGHT]-(:Customer)
            WITH m, 
                 count(r) AS tickets_sold,
                 avg(r.price) AS average_price,
                 sum(r.price) AS total_revenue
            RETURN m.name AS match_name,
                   tickets_sold,
                   CASE WHEN average_price IS NULL THEN 0 ELSE average_price END AS average_price,
                   CASE WHEN total_revenue IS NULL THEN 0 ELSE total_revenue END AS total_revenue
            ORDER BY m.scheduled_at";

        var result = await _graphDbContext.RunAsync(query, new { sector = sector });
        var matchAnalysis = new List<SectorMatchAnalysisDto>();
        var totalTicketsForSector = 0;

        // Prvo čitamo sve rezultate u listu
        var tempResults = new List<(string matchName, int ticketsSold, decimal averagePrice, decimal totalRevenue)>();

        await foreach (var record in result)
        {
            var ticketsSold = record["tickets_sold"].As<int>();
            totalTicketsForSector += ticketsSold;
            
            tempResults.Add((
                record["match_name"].As<string>(),
                ticketsSold,
                record["average_price"].As<decimal>(),
                record["total_revenue"].As<decimal>()
            ));
        }

        // Sada kreiramo finalne rezultate sa procentima
        foreach (var temp in tempResults)
        {
            var salesPercentage = totalTicketsForSector > 0 ? (double)temp.ticketsSold / totalTicketsForSector * 100 : 0;
            
            matchAnalysis.Add(new SectorMatchAnalysisDto
            {
                MatchName = temp.matchName,
                TicketsSold = temp.ticketsSold,
                AveragePrice = temp.averagePrice,
                TotalRevenue = temp.totalRevenue,
                SalesPercentage = Math.Round(salesPercentage, 2)
            });
        }

        return matchAnalysis;
    }
}