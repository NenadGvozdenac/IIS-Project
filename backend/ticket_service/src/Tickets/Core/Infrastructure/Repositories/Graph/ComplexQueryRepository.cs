using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;
using ticket_service.src.Tickets.Core.Infrastructure.Database;
using Neo4j.Driver;

namespace ticket_service.src.Tickets.Core.Infrastructure.Repositories.Graph;

public class ComplexQueryRepository : IGraphComplexQueryRepository
{
    private readonly IGraphDatabaseContext _graphDbContext;

    public ComplexQueryRepository(IGraphDatabaseContext graphDbContext)
    {
        _graphDbContext = graphDbContext;
    }

    public async Task<List<MatchRevenueDto>> GetMatchesWithRevenueAboveThreshold(int minTicketsSold)
    {
        var query = @"
            MATCH (m:Match)<-[:IS_FOR_MATCH]-(t:IndividualTicket)<-[r:BOUGHT]-(:Customer)
            WITH m, count(r) AS broj_prodatih, sum(r.price) AS ukupna_zarada
            WHERE broj_prodatih > $minTicketsSold
            RETURN m.name AS mec, broj_prodatih, ukupna_zarada
            ORDER BY ukupna_zarada DESC";

        var parameters = new { minTicketsSold = minTicketsSold };
        var result = await _graphDbContext.RunAsync(query, parameters);
        var matches = new List<MatchRevenueDto>();

        await foreach (var record in result)
        {
            matches.Add(new MatchRevenueDto
            {
                MatchName = record["mec"].As<string>(),
                TicketsSold = record["broj_prodatih"].As<int>(),
                TotalRevenue = record["ukupna_zarada"].As<decimal>()
            });
        }

        return matches;
    }

    public async Task<List<CustomerSpendingDto>> GetCustomersWithHighSpending(decimal minSpending)
    {
        var query = @"
            MATCH (c:Customer)-[r:BOUGHT]->(t:IndividualTicket)
            WITH c, sum(r.price) AS ukupno_potroseno, count(r) AS broj_karata
            WHERE ukupno_potroseno > $minSpending
            RETURN c.name + ' ' + c.surname AS kupac, broj_karata, ukupno_potroseno
            ORDER BY ukupno_potroseno DESC";

        var parameters = new { minSpending = minSpending };
        var result = await _graphDbContext.RunAsync(query, parameters);
        var customers = new List<CustomerSpendingDto>();

        await foreach (var record in result)
        {
            customers.Add(new CustomerSpendingDto
            {
                CustomerName = record["kupac"].As<string>(),
                TicketCount = record["broj_karata"].As<int>(),
                TotalSpent = record["ukupno_potroseno"].As<decimal>()
            });
        }

        return customers;
    }

    public async Task<SectorSalesResultDto> GetSectorSalesForMatch(int matchId, int minTicketsSold)
    {
        var query = @"
            MATCH (m:Match)
            WHERE id(m) = $matchId
            MATCH (c:Customer)-[:BOUGHT]->(t:IndividualTicket)-[:IS_FOR_MATCH]->(m)
            MATCH (t)-[:IS_FOR_SEAT]->(s:Seat)
            WITH m.name AS matchName, s.direction AS sektor, count(t) AS broj_prodatih
            WHERE broj_prodatih > $minTicketsSold
            RETURN matchName, sektor, broj_prodatih
            ORDER BY broj_prodatih DESC";

        var parameters = new { matchId = matchId, minTicketsSold = minTicketsSold };
        var result = await _graphDbContext.RunAsync(query, parameters);
        var sectors = new List<SectorSalesDto>();
        string matchName = string.Empty;

        await foreach (var record in result)
        {
            if (string.IsNullOrEmpty(matchName))
            {
                matchName = record["matchName"].As<string>();
            }
            
            sectors.Add(new SectorSalesDto
            {
                Sector = record["sektor"].As<string>(),
                TicketsSold = record["broj_prodatih"].As<int>()
            });
        }

        // If no sectors found, still get match name
        if (sectors.Count == 0)
        {
            var matchQuery = "MATCH (m:Match) WHERE id(m) = $matchId RETURN m.name AS matchName";
            var matchResult = await _graphDbContext.RunAsync(matchQuery, new { matchId = matchId });
            await foreach (var record in matchResult)
            {
                matchName = record["matchName"].As<string>();
                break;
            }
        }

        return new SectorSalesResultDto
        {
            MatchName = matchName,
            Sectors = sectors
        };
    }

    public async Task<List<CustomerMatchesDto>> GetCustomersWithMultipleMatches()
    {
        var query = @"
            MATCH (c:Customer)-[:BOUGHT]->(t:IndividualTicket)-[:IS_FOR_MATCH]->(m:Match)
            WITH c, collect(DISTINCT m.name) AS mecevi, count(DISTINCT m) AS broj_meceva
            WHERE broj_meceva > 1
            RETURN c.name + ' ' + c.surname AS kupac, broj_meceva, mecevi
            ORDER BY broj_meceva DESC";

        var result = await _graphDbContext.RunAsync(query, new { });
        var customers = new List<CustomerMatchesDto>();

        await foreach (var record in result)
        {
            customers.Add(new CustomerMatchesDto
            {
                CustomerName = record["kupac"].As<string>(),
                MatchCount = record["broj_meceva"].As<int>(),
                Matches = record["mecevi"].As<List<string>>()
            });
        }

        return customers;
    }

    public async Task<List<MatchAveragePriceDto>> GetMatchesWithAveragePrice(int minTicketsSold)
    {
        var query = @"
            MATCH (m:Match)<-[:IS_FOR_MATCH]-(t:IndividualTicket)<-[r:BOUGHT]-(:Customer)
            WITH m, count(r) AS broj_prodatih, avg(r.price) AS prosecna_cena
            WHERE broj_prodatih > $minTicketsSold
            RETURN m.name AS mec, broj_prodatih, prosecna_cena
            ORDER BY prosecna_cena DESC";

        var parameters = new { minTicketsSold = minTicketsSold };
        var result = await _graphDbContext.RunAsync(query, parameters);
        var matches = new List<MatchAveragePriceDto>();

        await foreach (var record in result)
        {
            matches.Add(new MatchAveragePriceDto
            {
                MatchName = record["mec"].As<string>(),
                TicketsSold = record["broj_prodatih"].As<int>(),
                AveragePrice = record["prosecna_cena"].As<decimal>()
            });
        }

        return matches;
    }
}