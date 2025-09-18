using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;
using ticket_service.src.Tickets.Core.Infrastructure.Database;
using Neo4j.Driver;
using ticket_service.src.Tickets.Core.Domain.Entities.Graph;

namespace ticket_service.src.Tickets.Core.Infrastructure.Repositories.Graph;

public class IsForMatchRelationshipRepository : IGraphIsForMatchRelationshipRepository
{
    private readonly IGraphDatabaseContext _graphDbContext;

    public IsForMatchRelationshipRepository(IGraphDatabaseContext graphDbContext)
    {
        _graphDbContext = graphDbContext;
    }

    public async Task<IsForMatchRelationship?> CreateIsForMatchRelationship(int individualTicketId, int matchId)
    {
        // First check if the ticket is already assigned to a match
        var checkQuery = @"
            MATCH (t:IndividualTicket) WHERE id(t) = $ticketId
            OPTIONAL MATCH (t)-[existing:IS_FOR_MATCH]->(:Match)
            RETURN existing IS NOT NULL as alreadyAssigned";

        var checkParameters = new { ticketId = individualTicketId };
        var checkResult = await _graphDbContext.RunAsync(checkQuery, checkParameters);

        await foreach (var record in checkResult)
        {
            if (record["alreadyAssigned"].As<bool>())
            {
                return null; // Ticket is already assigned to a match
            }
        }

        var query = @"
            MATCH (t:IndividualTicket) WHERE id(t) = $ticketId
            MATCH (m:Match) WHERE id(m) = $matchId
            CREATE (t)-[r:IS_FOR_MATCH]->(m)
            RETURN id(t) as ticketId, id(m) as matchId, elementId(r) as relationshipElementId";

        var parameters = new
        {
            ticketId = individualTicketId,
            matchId = matchId
        };

        var result = await _graphDbContext.RunAsync(query, parameters);

        await foreach (var record in result)
        {
            return new IsForMatchRelationship
            {
                IndividualTicketId = record["ticketId"].As<int>(),
                MatchId = record["matchId"].As<int>(),
                RelationshipElementId = record["relationshipElementId"].As<string>()
            };
        }

        return null;
    }

    public async Task<List<IsForMatchRelationship>> GetIsForMatchRelationshipsByTicketId(int individualTicketId)
    {
        var query = @"
            MATCH (t:IndividualTicket)-[r:IS_FOR_MATCH]->(m:Match) 
            WHERE id(t) = $ticketId
            RETURN id(t) as ticketId, id(m) as matchId, elementId(r) as relationshipElementId";

        var parameters = new { ticketId = individualTicketId };
        var result = await _graphDbContext.RunAsync(query, parameters);
        var relationships = new List<IsForMatchRelationship>();

        await foreach (var record in result)
        {
            relationships.Add(new IsForMatchRelationship
            {
                IndividualTicketId = record["ticketId"].As<int>(),
                MatchId = record["matchId"].As<int>(),
                RelationshipElementId = record["relationshipElementId"].As<string>()
            });
        }

        return relationships;
    }

    public async Task<List<IsForMatchRelationship>> GetIsForMatchRelationshipsByMatchId(int matchId)
    {
        var query = @"
            MATCH (t:IndividualTicket)-[r:IS_FOR_MATCH]->(m:Match) 
            WHERE id(m) = $matchId
            RETURN id(t) as ticketId, id(m) as matchId, elementId(r) as relationshipElementId";

        var parameters = new { matchId = matchId };
        var result = await _graphDbContext.RunAsync(query, parameters);
        var relationships = new List<IsForMatchRelationship>();

        await foreach (var record in result)
        {
            relationships.Add(new IsForMatchRelationship
            {
                IndividualTicketId = record["ticketId"].As<int>(),
                MatchId = record["matchId"].As<int>(),
                RelationshipElementId = record["relationshipElementId"].As<string>()
            });
        }

        return relationships;
    }

    public async Task<IsForMatchRelationship?> GetIsForMatchRelationship(int individualTicketId, int matchId)
    {
        var query = @"
            MATCH (t:IndividualTicket)-[r:IS_FOR_MATCH]->(m:Match) 
            WHERE id(t) = $ticketId AND id(m) = $matchId
            RETURN id(t) as ticketId, id(m) as matchId, elementId(r) as relationshipElementId";

        var parameters = new { ticketId = individualTicketId, matchId = matchId };
        var result = await _graphDbContext.RunAsync(query, parameters);

        await foreach (var record in result)
        {
            return new IsForMatchRelationship
            {
                IndividualTicketId = record["ticketId"].As<int>(),
                MatchId = record["matchId"].As<int>(),
                RelationshipElementId = record["relationshipElementId"].As<string>()
            };
        }

        return null;
    }

    public async Task DeleteIsForMatchRelationship(int individualTicketId, int matchId)
    {
        var query = @"
            MATCH (t:IndividualTicket)-[r:IS_FOR_MATCH]->(m:Match) 
            WHERE id(t) = $ticketId AND id(m) = $matchId
            DELETE r";

        var parameters = new { ticketId = individualTicketId, matchId = matchId };
        await _graphDbContext.RunAsync(query, parameters);
    }
}