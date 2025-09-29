using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;
using ticket_service.src.Tickets.Core.Infrastructure.Database;
using Neo4j.Driver;
using ticket_service.src.Tickets.Core.Domain.Entities.Graph;

namespace ticket_service.src.Tickets.Core.Infrastructure.Repositories.Graph;

public class IsForSeatRelationshipRepository : IGraphIsForSeatRelationshipRepository
{
    private readonly IGraphDatabaseContext _graphDbContext;

    public IsForSeatRelationshipRepository(IGraphDatabaseContext graphDbContext)
    {
        _graphDbContext = graphDbContext;
    }

    public async Task<IsForSeatRelationship?> CreateIsForSeatRelationship(int individualTicketId, int seatId)
    {
        // First check if the ticket is already assigned to a seat
        var checkQuery = @"
            MATCH (t:IndividualTicket) WHERE id(t) = $ticketId
            OPTIONAL MATCH (t)-[existing:IS_FOR_SEAT]->(:Seat)
            RETURN existing IS NOT NULL as alreadyAssigned";

        var checkParameters = new { ticketId = individualTicketId };
        var checkResult = await _graphDbContext.RunAsync(checkQuery, checkParameters);

        await foreach (var record in checkResult)
        {
            if (record["alreadyAssigned"].As<bool>())
            {
                return null; // Ticket is already assigned to a seat
            }
        }

        var query = @"
            MATCH (t:IndividualTicket) WHERE id(t) = $ticketId
            MATCH (s:Seat) WHERE id(s) = $seatId
            CREATE (t)-[r:IS_FOR_SEAT]->(s)
            RETURN id(t) as ticketId, id(s) as seatId, elementId(r) as relationshipElementId";

        var parameters = new
        {
            ticketId = individualTicketId,
            seatId = seatId
        };

        var result = await _graphDbContext.RunAsync(query, parameters);

        await foreach (var record in result)
        {
            return new IsForSeatRelationship
            {
                IndividualTicketId = record["ticketId"].As<int>(),
                SeatId = record["seatId"].As<int>(),
                RelationshipElementId = record["relationshipElementId"].As<string>()
            };
        }

        return null;
    }

    public async Task<List<IsForSeatRelationship>> GetIsForSeatRelationshipsByTicketId(int individualTicketId)
    {
        var query = @"
            MATCH (t:IndividualTicket)-[r:IS_FOR_SEAT]->(s:Seat) 
            WHERE id(t) = $ticketId
            RETURN id(t) as ticketId, id(s) as seatId, elementId(r) as relationshipElementId";

        var parameters = new { ticketId = individualTicketId };
        var result = await _graphDbContext.RunAsync(query, parameters);
        var relationships = new List<IsForSeatRelationship>();

        await foreach (var record in result)
        {
            relationships.Add(new IsForSeatRelationship
            {
                IndividualTicketId = record["ticketId"].As<int>(),
                SeatId = record["seatId"].As<int>(),
                RelationshipElementId = record["relationshipElementId"].As<string>()
            });
        }

        return relationships;
    }

    public async Task<List<IsForSeatRelationship>> GetIsForSeatRelationshipsBySeatId(int seatId)
    {
        var query = @"
            MATCH (t:IndividualTicket)-[r:IS_FOR_SEAT]->(s:Seat) 
            WHERE id(s) = $seatId
            RETURN id(t) as ticketId, id(s) as seatId, elementId(r) as relationshipElementId";

        var parameters = new { seatId = seatId };
        var result = await _graphDbContext.RunAsync(query, parameters);
        var relationships = new List<IsForSeatRelationship>();

        await foreach (var record in result)
        {
            relationships.Add(new IsForSeatRelationship
            {
                IndividualTicketId = record["ticketId"].As<int>(),
                SeatId = record["seatId"].As<int>(),
                RelationshipElementId = record["relationshipElementId"].As<string>()
            });
        }

        return relationships;
    }

    public async Task<IsForSeatRelationship?> GetIsForSeatRelationship(int individualTicketId, int seatId)
    {
        var query = @"
            MATCH (t:IndividualTicket)-[r:IS_FOR_SEAT]->(s:Seat) 
            WHERE id(t) = $ticketId AND id(s) = $seatId
            RETURN id(t) as ticketId, id(s) as seatId, elementId(r) as relationshipElementId";

        var parameters = new { ticketId = individualTicketId, seatId = seatId };
        var result = await _graphDbContext.RunAsync(query, parameters);

        await foreach (var record in result)
        {
            return new IsForSeatRelationship
            {
                IndividualTicketId = record["ticketId"].As<int>(),
                SeatId = record["seatId"].As<int>(),
                RelationshipElementId = record["relationshipElementId"].As<string>()
            };
        }

        return null;
    }

    public async Task DeleteIsForSeatRelationship(int individualTicketId, int seatId)
    {
        var query = @"
            MATCH (t:IndividualTicket)-[r:IS_FOR_SEAT]->(s:Seat) 
            WHERE id(t) = $ticketId AND id(s) = $seatId
            DELETE r";

        var parameters = new { ticketId = individualTicketId, seatId = seatId };
        await _graphDbContext.RunAsync(query, parameters);
    }
}