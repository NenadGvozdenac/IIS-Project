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

    public async Task<Seat?> CreateSeat(Seat seat)
    {
        var query = @"
            CREATE (s:Seat {
                name: $name,
                row: $row,
                number: $number,
                direction: $direction
            }) RETURN id(s) as nodeId, elementId(s) as elementId,
                   s.name as name, s.row as row, s.number as number, s.direction as direction";

        var parameters = new
        {
            name = seat.Name,
            row = seat.Row,
            number = seat.Number,
            direction = seat.Direction
        };

        var result = await _graphDbContext.RunAsync(query, parameters);

        await foreach (var record in result)
        {
            return new Seat
            {
                Id = record["nodeId"].As<int>(),
                ElementId = record["elementId"].As<string>(),
                Name = record["name"].As<string>(),
                Row = record["row"].As<int>(),
                Number = record["number"].As<int>(),
                Direction = record["direction"].As<string>()
            };
        }
        
        return null;
    }

    public async Task DeleteSeat(int id)
    {
        var query = @"
            MATCH (s:Seat)
            WHERE id(s) = $id
            DETACH DELETE s";

        var parameters = new { id };

        await _graphDbContext.RunAsync(query, parameters);
    }

    public async Task<List<Seat>> GetAllSeats()
    {
        var query = @"
            MATCH (s:Seat)
            RETURN id(s) as nodeId, elementId(s) as elementId,
                   s.name as name, s.row as row, s.number as number, s.direction as direction
            ORDER BY s.name";

        var result = await _graphDbContext.RunAsync(query);
        var seats = new List<Seat>();

        await foreach (var record in result)
        {
            seats.Add(new Seat
            {
                Id = record["nodeId"].As<int>(),
                ElementId = record["elementId"].As<string>(),
                Name = record["name"].As<string>(),
                Row = record["row"].As<int>(),
                Number = record["number"].As<int>(),
                Direction = record["direction"].As<string>()
            });
        }

        return seats;
    }

    public async Task<Seat?> GetSeatById(int id)
    {
        var query = @"
            MATCH (s:Seat)
            WHERE id(s) = $id
            RETURN id(s) as nodeId, elementId(s) as elementId, 
                   s.name as name, s.row as row, s.number as number, s.direction as direction";

        var parameters = new { id };
        var result = await _graphDbContext.RunAsync(query, parameters);

        await foreach (var record in result)
        {
            return new Seat
            {
                Id = record["nodeId"].As<int>(),
                ElementId = record["elementId"].As<string>(),
                Name = record["name"].As<string>(),
                Row = record["row"].As<int>(),
                Number = record["number"].As<int>(),
                Direction = record["direction"].As<string>()
            };
        }

        return null;
    }

    public async Task<Seat?> GetSeatByName(string name)
    {
        var query = @"
            MATCH (s:Seat {name: $name})
            RETURN id(s) as nodeId, elementId(s) as elementId,
                   s.name as name, s.row as row, s.number as number, s.direction as direction";

        var parameters = new { name };
        var result = await _graphDbContext.RunAsync(query, parameters);

        await foreach (var record in result)
        {
            return new Seat
            {
                Id = record["nodeId"].As<int>(),
                ElementId = record["elementId"].As<string>(),
                Name = record["name"].As<string>(),
                Row = record["row"].As<int>(),
                Number = record["number"].As<int>(),
                Direction = record["direction"].As<string>()
            };
        }

        return null;
    }

    public async Task<Seat?> UpdateSeat(int id, Seat seat)
    {
        var query = @"
            MATCH (s:Seat)
            WHERE id(s) = $id
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

        var result = await _graphDbContext.RunAsync(query, parameters);

        await foreach (var record in result)
        {
            return new Seat
            {
                Id = record["nodeId"].As<int>(),
                ElementId = record["elementId"].As<string>(),
                Name = record["name"].As<string>(),
                Row = record["row"].As<int>(),
                Number = record["number"].As<int>(),
                Direction = record["direction"].As<string>()
            };
        }

        return null;
    }
}