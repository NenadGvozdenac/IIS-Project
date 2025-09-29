using Neo4j.Driver;

namespace ticket_service.src.Tickets.Core.Infrastructure.Database;

public interface IGraphDatabaseContext : IDisposable
{
    public Task<IResultCursor> RunAsync(string query, object parameters);
    public Task<IResultCursor> RunAsync(string query);
}