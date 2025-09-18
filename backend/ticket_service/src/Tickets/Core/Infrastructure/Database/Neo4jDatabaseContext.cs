using Neo4j.Driver;

namespace ticket_service.src.Tickets.Core.Infrastructure.Database;

public class Neo4jDatabaseContext : IGraphDatabaseContext
{
    private readonly IDriver _driver;
    private readonly IAsyncSession _session;

    public Neo4jDatabaseContext(IConfiguration configuration)
    {
        var uri = configuration["Neo4j:Uri"];
        var username = configuration["Neo4j:Username"];
        var password = configuration["Neo4j:Password"];

        _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(username, password));
        _session = _driver.AsyncSession();
    }

    public async Task<IResultCursor> RunAsync(string query, object parameters)
    {
        return await _session.RunAsync(query, parameters);
    }

    public async Task<IResultCursor> RunAsync(string query)
    {
        return await _session.RunAsync(query);
    }

    public void Dispose()
    {
        _session?.Dispose();
        _driver?.Dispose();
    }
}