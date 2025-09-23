using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;
using ticket_service.src.Tickets.Core.Infrastructure.Database;
using Neo4j.Driver;
using ticket_service.src.Tickets.Core.Domain.Entities.Graph;

namespace ticket_service.src.Tickets.Core.Infrastructure.Repositories.Graph;

public class BoughtRelationshipRepository : IGraphBoughtRelationshipRepository
{
    private readonly IGraphDatabaseContext _graphDbContext;

    public BoughtRelationshipRepository(IGraphDatabaseContext graphDbContext)
    {
        _graphDbContext = graphDbContext;
    }

    public async Task<BoughtRelationship?> CreateBoughtRelationship(int customerId, int individualTicketId, DateTime purchasedAt, decimal price)
    {
        // First check if the ticket is already bought by someone
        var checkQuery = @"
            MATCH (t:IndividualTicket) WHERE id(t) = $ticketId
            OPTIONAL MATCH (:Customer)-[existing:BOUGHT]->(t)
            RETURN existing IS NOT NULL as alreadyBought";

        var checkParameters = new { ticketId = individualTicketId };
        var checkResult = await _graphDbContext.RunAsync(checkQuery, checkParameters);

        await foreach (var record in checkResult)
        {
            if (record["alreadyBought"].As<bool>())
            {
                return null; // Ticket is already bought
            }
        }

        var query = @"
            MATCH (c:Customer) WHERE id(c) = $customerId
            MATCH (t:IndividualTicket) WHERE id(t) = $ticketId
            CREATE (c)-[r:BOUGHT {purchased_at: $purchasedAt, price: $price}]->(t)
            RETURN id(c) as customerId, id(t) as ticketId, elementId(r) as relationshipElementId,
                   r.purchased_at as purchasedAt, r.price as price";

        var parameters = new
        {
            customerId = customerId,
            ticketId = individualTicketId,
            purchasedAt = purchasedAt,
            price = price
        };

        var result = await _graphDbContext.RunAsync(query, parameters);

        await foreach (var record in result)
        {
            var zonedDateTime = record["purchasedAt"].As<ZonedDateTime>();
            return new BoughtRelationship
            {
                CustomerId = record["customerId"].As<int>(),
                IndividualTicketId = record["ticketId"].As<int>(),
                RelationshipElementId = record["relationshipElementId"].As<string>(),
                PurchasedAt = zonedDateTime.ToDateTimeOffset().DateTime,
                Price = record["price"].As<decimal>()
            };
        }

        return null;
    }

    public async Task<List<BoughtRelationship>> GetBoughtRelationshipsByCustomerId(int customerId)
    {
        var query = @"
            MATCH (c:Customer)-[r:BOUGHT]->(t:IndividualTicket) 
            WHERE id(c) = $customerId
            RETURN id(c) as customerId, id(t) as ticketId, elementId(r) as relationshipElementId,
                   r.purchased_at as purchasedAt, r.price as price";

        var parameters = new { customerId = customerId };
        var result = await _graphDbContext.RunAsync(query, parameters);
        var relationships = new List<BoughtRelationship>();

        await foreach (var record in result)
        {
            var zonedDateTime = record["purchasedAt"].As<ZonedDateTime>();
            relationships.Add(new BoughtRelationship
            {
                CustomerId = record["customerId"].As<int>(),
                IndividualTicketId = record["ticketId"].As<int>(),
                RelationshipElementId = record["relationshipElementId"].As<string>(),
                PurchasedAt = zonedDateTime.ToDateTimeOffset().DateTime,
                Price = record["price"].As<decimal>()
            });
        }

        return relationships;
    }

    public async Task<List<BoughtRelationship>> GetBoughtRelationshipsByTicketId(int individualTicketId)
    {
        var query = @"
            MATCH (c:Customer)-[r:BOUGHT]->(t:IndividualTicket) 
            WHERE id(t) = $ticketId
            RETURN id(c) as customerId, id(t) as ticketId, elementId(r) as relationshipElementId,
                   r.purchased_at as purchasedAt, r.price as price";

        var parameters = new { ticketId = individualTicketId };
        var result = await _graphDbContext.RunAsync(query, parameters);
        var relationships = new List<BoughtRelationship>();

        await foreach (var record in result)
        {
            var zonedDateTime = record["purchasedAt"].As<ZonedDateTime>();
            relationships.Add(new BoughtRelationship
            {
                CustomerId = record["customerId"].As<int>(),
                IndividualTicketId = record["ticketId"].As<int>(),
                RelationshipElementId = record["relationshipElementId"].As<string>(),
                PurchasedAt = zonedDateTime.ToDateTimeOffset().DateTime,
                Price = record["price"].As<decimal>()
            });
        }

        return relationships;
    }

    public async Task<BoughtRelationship?> GetBoughtRelationship(int customerId, int individualTicketId)
    {
        var query = @"
            MATCH (c:Customer)-[r:BOUGHT]->(t:IndividualTicket) 
            WHERE id(c) = $customerId AND id(t) = $ticketId
            RETURN id(c) as customerId, id(t) as ticketId, elementId(r) as relationshipElementId,
                   r.purchased_at as purchasedAt, r.price as price";

        var parameters = new { customerId = customerId, ticketId = individualTicketId };
        var result = await _graphDbContext.RunAsync(query, parameters);

        await foreach (var record in result)
        {
            var zonedDateTime = record["purchasedAt"].As<ZonedDateTime>();
            return new BoughtRelationship
            {
                CustomerId = record["customerId"].As<int>(),
                IndividualTicketId = record["ticketId"].As<int>(),
                RelationshipElementId = record["relationshipElementId"].As<string>(),
                PurchasedAt = zonedDateTime.ToDateTimeOffset().DateTime,
                Price = record["price"].As<decimal>()
            };
        }

        return null;
    }

    public async Task DeleteBoughtRelationship(int customerId, int individualTicketId)
    {
        var query = @"
            MATCH (c:Customer)-[r:BOUGHT]->(t:IndividualTicket) 
            WHERE id(c) = $customerId AND id(t) = $ticketId
            DELETE r";

        var parameters = new { customerId = customerId, ticketId = individualTicketId };
        await _graphDbContext.RunAsync(query, parameters);
    }

    public async Task<bool> IsTicketAlreadyBought(int individualTicketId)
    {
        var query = @"
            MATCH (t:IndividualTicket) WHERE id(t) = $ticketId
            OPTIONAL MATCH (:Customer)-[r:BOUGHT]->(t)
            RETURN r IS NOT NULL as isBought";

        var parameters = new { ticketId = individualTicketId };
        var result = await _graphDbContext.RunAsync(query, parameters);

        await foreach (var record in result)
        {
            return record["isBought"].As<bool>();
        }

        return false;
    }
}