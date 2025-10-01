using Cassandra;
using Cassandra.Mapping;
using Travel_Service.src.Travels.Core.Application.Interfaces.ColumnarRepositories;

namespace Travel_Service.src.Travels.Core.Infrastructure.ColumnarRepositories;

public abstract class BaseColumnarRepository<T> : IColumnarRepository<T> where T : class
{
    protected readonly Cassandra.ISession _session;
    protected readonly Cassandra.Mapping.IMapper _mapper;
    protected readonly string _tableName;

    protected BaseColumnarRepository(Cassandra.ISession session, Cassandra.Mapping.IMapper mapper, string tableName)
    {
        _session = session;
        _mapper = mapper;
        _tableName = tableName;
    }

    public virtual async Task<T?> GetByIdAsync(params object[] keys)
    {
        try
        {
            var query = $"SELECT * FROM {_tableName} WHERE {GetWhereClause(keys.Length)} ALLOW FILTERING";
            Console.WriteLine($"Executing query: {query} with keys: {string.Join(", ", keys)}");
            var result = await _mapper.SingleOrDefaultAsync<T>(query, keys);
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting entity by ID from {_tableName}", ex);
        }
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        try
        {
            var query = $"SELECT * FROM {_tableName}";
            var result = await _mapper.FetchAsync<T>(query);
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting all entities from {_tableName}", ex);
        }
    }

    public virtual async Task<IEnumerable<T>> GetByPartitionKeyAsync(object partitionKey)
    {
        try
        {
            var query = $"SELECT * FROM {_tableName} WHERE {GetPrimaryKeyColumn()} = ?";
            var result = await _mapper.FetchAsync<T>(query, partitionKey);
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting entities by partition key from {_tableName}", ex);
        }
    }

    public virtual async Task<bool> CreateAsync(T entity)
    {
        try
        {
            Console.WriteLine($"Creating entity in {entity.GetType().Name} table");
            await _mapper.InsertAsync(entity);
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error creating entity in {_tableName}", ex);
        }
    }

    public virtual async Task<bool> UpdateAsync(T entity)
    {
        try
        {
            await _mapper.UpdateAsync(entity);
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating entity in {_tableName}", ex);
        }
    }

    public virtual async Task<bool> DeleteAsync(params object[] keys)
    {
        try
        {
            var query = $"DELETE FROM {_tableName} WHERE {GetWhereClause(keys.Length)}";
            Console.WriteLine($"Executing query: {query} with keys: {string.Join(", ", keys)}");
            await _session.ExecuteAsync(new SimpleStatement(query, keys));
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting entity from {_tableName}", ex);
        }
    }

    public virtual async Task<bool> ExistsAsync(params object[] keys)
    {
        try
        {
            var entity = await GetByIdAsync(keys);
            return entity != null;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error checking entity existence in {_tableName}", ex);
        }
    }

    protected abstract string GetPrimaryKeyColumn();
    protected abstract string GetWhereClause(int keyCount);
    protected abstract object[] GetEntityKeyValues(T entity);
}