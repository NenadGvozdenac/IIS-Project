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

    public async Task DeleteMatch(int id)
    {
        var query = @"
            MATCH (m:Match)
            WHERE id(m) = $id
            DETACH DELETE m";

        var parameters = new { id };

        await _graphDbContext.RunAsync(query, parameters);
    }

    public async Task<List<Match>> GetAllMatches()
    {
        var query = @"
            MATCH (m:Match)
            RETURN id(m) as nodeId, elementId(m) as elementId,
                   m.name as name, m.scheduled_at as scheduledAt, m.type as type,
                   m.state as state, m.city as city, m.hall as hall, 
                   m.is_in_our_hall as isInOurHall";

        var result = await _graphDbContext.RunAsync(query);
        var matches = new List<Match>();

        await foreach (var record in result)
        {
            matches.Add(new Match
            {
                Id = record["nodeId"].As<long>().ToString(),
                ElementId = record["elementId"].As<string>(),
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

    public async Task<Match?> GetMatchById(int id)
    {
        var query = @"
            MATCH (m:Match)
            WHERE id(m) = $id
            RETURN id(m) as nodeId, elementId(m) as elementId,
                   m.name as name, m.scheduled_at as scheduledAt, m.type as type,
                   m.state as state, m.city as city, m.hall as hall, 
                   m.is_in_our_hall as isInOurHall";

        var parameters = new { id };
        var result = await _graphDbContext.RunAsync(query, parameters);

        await foreach (var record in result)
        {
            return new Match
            {
                Id = record["nodeId"].As<long>().ToString(),
                ElementId = record["elementId"].As<string>(),
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

    public async Task<Match?> GetMatchByName(string name)
    {
        var query = @"
            MATCH (m:Match {name: $name})
            RETURN id(m) as nodeId, elementId(m) as elementId,
                   m.name as name, m.scheduled_at as scheduledAt, m.type as type,
                   m.state as state, m.city as city, m.hall as hall, 
                   m.is_in_our_hall as isInOurHall";

        var parameters = new { name };
        var result = await _graphDbContext.RunAsync(query, parameters);

        await foreach (var record in result)
        {
            return new Match
            {
                Id = record["nodeId"].As<long>().ToString(),
                ElementId = record["elementId"].As<string>(),
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

    public async Task UpdateMatch(int id, Match match)
    {
        var query = @"
            MATCH (m:Match)
            WHERE id(m) = $id
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