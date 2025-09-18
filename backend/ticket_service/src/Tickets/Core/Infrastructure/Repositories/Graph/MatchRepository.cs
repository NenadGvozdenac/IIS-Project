using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;
using ticket_service.src.Tickets.Core.Infrastructure.Database;
using Neo4j.Driver;
using ticket_service.src.Tickets.Core.Domain.Entities.Graph;

namespace ticket_service.src.Tickets.Core.Infrastructure.Repositories.Graph;

public class MatchRepository : IGraphMatchRepository
{
    private readonly IGraphDatabaseContext _graphDbContext;

    public MatchRepository(IGraphDatabaseContext graphDbContext)
    {
        _graphDbContext = graphDbContext;
    }

    public async Task CreateMatch(Match match)
    {
        var query = @"
            CREATE (m:Match {
                name: $name,
                scheduled_at: datetime($scheduledAt),
                type: $type,
                state: $state,
                city: $city,
                hall: $hall,
                is_in_our_hall: $isInOurHall
            })";

        var parameters = new
        {
            name = match.Name,
            scheduledAt = match.ScheduledAt.ToString("yyyy-MM-ddTHH:mm:ssK"),
            type = match.Type,
            state = match.State,
            city = match.City,
            hall = match.Hall,
            isInOurHall = match.IsInOurHall
        };

        await _graphDbContext.RunAsync(query, parameters);
    }

    public async Task DeleteMatch(string name)
    {
        var query = @"
            MATCH (m:Match {name: $name})
            DETACH DELETE m";

        var parameters = new { name };

        await _graphDbContext.RunAsync(query, parameters);
    }

    public async Task<List<Match>> GetAllMatches()
    {
        var query = @"
            MATCH (m:Match)
            RETURN m.name as name, m.scheduled_at as scheduledAt, m.type as type,
                   m.state as state, m.city as city, m.hall as hall, 
                   m.is_in_our_hall as isInOurHall";

        var result = await _graphDbContext.RunAsync(query);
        var matches = new List<Match>();

        await foreach (var record in result)
        {
            matches.Add(new Match
            {
                Name = record["name"].As<string>(),
                ScheduledAt = record["scheduledAt"].As<ZonedDateTime>().ToDateTimeOffset().DateTime,
                Type = record["type"].As<string>(),
                State = record["state"].As<string>(),
                City = record["city"].As<string>(),
                Hall = record["hall"].As<string>(),
                IsInOurHall = record["isInOurHall"].As<bool>()
            });
        }

        return matches;
    }

    public async Task<Match?> GetMatchByName(string name)
    {
        var query = @"
            MATCH (m:Match {name: $name})
            RETURN m.name as name, m.scheduled_at as scheduledAt, m.type as type,
                   m.state as state, m.city as city, m.hall as hall, 
                   m.is_in_our_hall as isInOurHall";

        var parameters = new { name };
        var result = await _graphDbContext.RunAsync(query, parameters);

        await foreach (var record in result)
        {
            return new Match
            {
                Name = record["name"].As<string>(),
                ScheduledAt = record["scheduledAt"].As<ZonedDateTime>().ToDateTimeOffset().DateTime,
                Type = record["type"].As<string>(),
                State = record["state"].As<string>(),
                City = record["city"].As<string>(),
                Hall = record["hall"].As<string>(),
                IsInOurHall = record["isInOurHall"].As<bool>()
            };
        }

        return null;
    }

    public async Task<List<Match>> GetMatchesByDateRange(DateTime startDate, DateTime endDate)
    {
        var query = @"
            MATCH (m:Match)
            WHERE m.scheduled_at >= datetime($startDate) AND m.scheduled_at <= datetime($endDate)
            RETURN m.name as name, m.scheduled_at as scheduledAt, m.type as type,
                   m.state as state, m.city as city, m.hall as hall, 
                   m.is_in_our_hall as isInOurHall
            ORDER BY m.scheduled_at";

        var parameters = new
        {
            startDate = startDate.ToString("yyyy-MM-ddTHH:mm:ssK"),
            endDate = endDate.ToString("yyyy-MM-ddTHH:mm:ssK")
        };

        var result = await _graphDbContext.RunAsync(query, parameters);
        var matches = new List<Match>();

        await foreach (var record in result)
        {
            matches.Add(new Match
            {
                Name = record["name"].As<string>(),
                ScheduledAt = record["scheduledAt"].As<ZonedDateTime>().ToDateTimeOffset().DateTime,
                Type = record["type"].As<string>(),
                State = record["state"].As<string>(),
                City = record["city"].As<string>(),
                Hall = record["hall"].As<string>(),
                IsInOurHall = record["isInOurHall"].As<bool>()
            });
        }

        return matches;
    }

    public async Task UpdateMatch(Match match)
    {
        var query = @"
            MATCH (m:Match {name: $name})
            SET m.scheduled_at = datetime($scheduledAt),
                m.type = $type,
                m.state = $state,
                m.city = $city,
                m.hall = $hall,
                m.is_in_our_hall = $isInOurHall";

        var parameters = new
        {
            name = match.Name,
            scheduledAt = match.ScheduledAt.ToString("yyyy-MM-ddTHH:mm:ssK"),
            type = match.Type,
            state = match.State,
            city = match.City,
            hall = match.Hall,
            isInOurHall = match.IsInOurHall
        };

        await _graphDbContext.RunAsync(query, parameters);
    }
}