using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;
using ticket_service.src.Tickets.Core.Infrastructure.Database;
using Neo4j.Driver;
using ticket_service.src.Tickets.Core.Domain.Entities.Graph;

namespace ticket_service.src.Tickets.Core.Infrastructure.Repositories.Graph;

public class SeatRepository : IGraphSeatRepository
{
    private readonly IGraphDatabaseContext _graphDbContext;

    public SeatRepository(IGraphDatabaseContext graphDbContext)
    {
        _graphDbContext = graphDbContext;
    }

    public async Task CreateSeat(Seat seat)
    {
        var query = @"
            CREATE (s:Seat {
                name: $name,
                row: $row,
                number: $number,
                direction: $direction
            })";

        var parameters = new
        {
            name = seat.Name,
            row = seat.Row,
            number = seat.Number,
            direction = seat.Direction
        };

        await _graphDbContext.RunAsync(query, parameters);
    }

    public async Task DeleteSeat(string name)
    {
        var query = @"
            MATCH (s:Seat {name: $name})
            DETACH DELETE s";

        var parameters = new { name };

        await _graphDbContext.RunAsync(query, parameters);
    }

    public async Task<List<Seat>> GetAllSeats()
    {
        var query = @"
            MATCH (s:Seat)
            RETURN s.name as name, s.row as row, s.number as number, s.direction as direction
            ORDER BY s.name";

        var result = await _graphDbContext.RunAsync(query);
        var seats = new List<Seat>();

        await foreach (var record in result)
        {
            seats.Add(new Seat
            {
                Name = record["name"].As<string>(),
                Row = record["row"].As<int>(),
                Number = record["number"].As<int>(),
                Direction = record["direction"].As<string>()
            });
        }

        return seats;
    }

    public async Task<Seat?> GetSeatByName(string name)
    {
        var query = @"
            MATCH (s:Seat {name: $name})
            RETURN s.name as name, s.row as row, s.number as number, s.direction as direction";

        var parameters = new { name };
        var result = await _graphDbContext.RunAsync(query, parameters);

        await foreach (var record in result)
        {
            return new Seat
            {
                Name = record["name"].As<string>(),
                Row = record["row"].As<int>(),
                Number = record["number"].As<int>(),
                Direction = record["direction"].As<string>()
            };
        }

        return null;
    }

    public async Task<List<Seat>> GetSeatsByDirection(string direction)
    {
        var query = @"
            MATCH (s:Seat {direction: $direction})
            RETURN s.name as name, s.row as row, s.number as number, s.direction as direction
            ORDER BY s.row, s.number";

        var parameters = new { direction };
        var result = await _graphDbContext.RunAsync(query, parameters);
        var seats = new List<Seat>();

        await foreach (var record in result)
        {
            seats.Add(new Seat
            {
                Name = record["name"].As<string>(),
                Row = record["row"].As<int>(),
                Number = record["number"].As<int>(),
                Direction = record["direction"].As<string>()
            });
        }

        return seats;
    }

    public async Task<List<Seat>> GetSeatsByRowRange(int minRow, int maxRow)
    {
        var query = @"
            MATCH (s:Seat)
            WHERE s.row >= $minRow AND s.row <= $maxRow
            RETURN s.name as name, s.row as row, s.number as number, s.direction as direction
            ORDER BY s.row, s.number";

        var parameters = new { minRow, maxRow };
        var result = await _graphDbContext.RunAsync(query, parameters);
        var seats = new List<Seat>();

        await foreach (var record in result)
        {
            seats.Add(new Seat
            {
                Name = record["name"].As<string>(),
                Row = record["row"].As<int>(),
                Number = record["number"].As<int>(),
                Direction = record["direction"].As<string>()
            });
        }

        return seats;
    }

    public async Task UpdateSeat(Seat seat)
    {
        var query = @"
            MATCH (s:Seat {name: $name})
            SET s.row = $row,
                s.number = $number,
                s.direction = $direction";

        var parameters = new
        {
            name = seat.Name,
            row = seat.Row,
            number = seat.Number,
            direction = seat.Direction
        };

        await _graphDbContext.RunAsync(query, parameters);
    }
}