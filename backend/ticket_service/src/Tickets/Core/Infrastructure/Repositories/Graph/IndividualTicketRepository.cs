using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;
using ticket_service.src.Tickets.Core.Infrastructure.Database;
using Neo4j.Driver;
using ticket_service.src.Tickets.Core.Domain.Entities.Graph;

namespace ticket_service.src.Tickets.Core.Infrastructure.Repositories.Graph;

public class IndividualTicketRepository : IGraphIndividualTicketRepository
{
    private readonly IGraphDatabaseContext _graphDbContext;

    public IndividualTicketRepository(IGraphDatabaseContext graphDbContext)
    {
        _graphDbContext = graphDbContext;
    }

    private static DateTime ConvertLocalDateToDateTime(LocalDate localDate)
    {
        return new DateTime(localDate.Year, localDate.Month, localDate.Day);
    }

    public async Task CreateIndividualTicket(IndividualTicket ticket)
    {
        var query = @"
            CREATE (t:IndividualTicket {
                name: $name,
                description: $description,
                type: $type,
                released_at: date($releasedAt),
                price: $price
            })";

        var parameters = new
        {
            name = ticket.Name,
            description = ticket.Description,
            type = ticket.Type,
            releasedAt = ticket.ReleasedAt.ToString("yyyy-MM-dd"),
            price = ticket.Price
        };

        await _graphDbContext.RunAsync(query, parameters);
    }

    public async Task DeleteIndividualTicket(string name)
    {
        var query = @"
            MATCH (t:IndividualTicket {name: $name})
            DETACH DELETE t";

        var parameters = new { name };

        await _graphDbContext.RunAsync(query, parameters);
    }

    public async Task<List<IndividualTicket>> GetAllIndividualTickets()
    {
        var query = @"
            MATCH (t:IndividualTicket)
            RETURN t.name as name, t.description as description, t.type as type,
                   t.released_at as releasedAt, t.price as price
            ORDER BY t.name";

        var result = await _graphDbContext.RunAsync(query);
        var tickets = new List<IndividualTicket>();

        await foreach (var record in result)
        {
            tickets.Add(new IndividualTicket
            {
                Name = record["name"].As<string>(),
                Description = record["description"].As<string>(),
                Type = record["type"].As<string>(),
                ReleasedAt = ConvertLocalDateToDateTime(record["releasedAt"].As<LocalDate>()),
                Price = record["price"].As<decimal>()
            });
        }

        return tickets;
    }

    public async Task<IndividualTicket?> GetIndividualTicketByName(string name)
    {
        var query = @"
            MATCH (t:IndividualTicket {name: $name})
            RETURN t.name as name, t.description as description, t.type as type,
                   t.released_at as releasedAt, t.price as price";

        var parameters = new { name };
        var result = await _graphDbContext.RunAsync(query, parameters);

        await foreach (var record in result)
        {
            return new IndividualTicket
            {
                Name = record["name"].As<string>(),
                Description = record["description"].As<string>(),
                Type = record["type"].As<string>(),
                ReleasedAt = ConvertLocalDateToDateTime(record["releasedAt"].As<LocalDate>()),
                Price = record["price"].As<decimal>()
            };
        }

        return null;
    }

    public async Task<List<IndividualTicket>> GetTicketsByDateRange(DateTime startDate, DateTime endDate)
    {
        var query = @"
            MATCH (t:IndividualTicket)
            WHERE t.released_at >= date($startDate) AND t.released_at <= date($endDate)
            RETURN t.name as name, t.description as description, t.type as type,
                   t.released_at as releasedAt, t.price as price
            ORDER BY t.released_at";

        var parameters = new
        {
            startDate = startDate.ToString("yyyy-MM-dd"),
            endDate = endDate.ToString("yyyy-MM-dd")
        };

        var result = await _graphDbContext.RunAsync(query, parameters);

        var tickets = new List<IndividualTicket>();
        await foreach (var record in result)
        {
            tickets.Add(new IndividualTicket
            {
                Name = record["name"].As<string>(),
                Description = record["description"].As<string>(),
                Type = record["type"].As<string>(),
                ReleasedAt = ConvertLocalDateToDateTime(record["releasedAt"].As<LocalDate>()),
                Price = record["price"].As<decimal>()
            });
        }

        return tickets;
    }

    public async Task<List<IndividualTicket>> GetTicketsByType(string type)
    {
        var query = @"
            MATCH (t:IndividualTicket {type: $type})
            RETURN t.name as name, t.description as description, t.type as type,
                   t.released_at as releasedAt, t.price as price
            ORDER BY t.name";

        var parameters = new { type };
        var result = await _graphDbContext.RunAsync(query, parameters);

        var tickets = new List<IndividualTicket>();
        await foreach (var record in result)
        {
            tickets.Add(new IndividualTicket
            {
                Name = record["name"].As<string>(),
                Description = record["description"].As<string>(),
                Type = record["type"].As<string>(),
                ReleasedAt = ConvertLocalDateToDateTime(record["releasedAt"].As<LocalDate>()),
                Price = record["price"].As<decimal>()
            });
        }

        return tickets;
    }

    public async Task UpdateIndividualTicket(IndividualTicket ticket)
    {
        var query = @"
            MATCH (t:IndividualTicket {name: $name})
            SET t.description = $description,
                t.type = $type,
                t.released_at = date($releasedAt),
                t.price = $price";

        var parameters = new
        {
            name = ticket.Name,
            description = ticket.Description,
            type = ticket.Type,
            releasedAt = ticket.ReleasedAt.ToString("yyyy-MM-dd"),
            price = ticket.Price
        };

        await _graphDbContext.RunAsync(query, parameters);
    }
}