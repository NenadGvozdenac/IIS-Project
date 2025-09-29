using Nest;
using scouting_service.src.Elasticsearch.Models;

namespace scouting_service.src.Elasticsearch.Services
{
    public interface IPlayerSearchService
    {
        Task<ISearchResponse<PlayerDocument>> SearchPlayersAsync(string searchTerm, int size = 20);
        Task<ISearchResponse<PlayerDocument>> AnalyzePlayerPerformanceByPositionAsync(string position, int size = 20);
        Task<ISearchResponse<PlayerDocument>> ComparePhysicalMetricsAsync(PhysicalMetricsFilter filter);
    }

    public interface ISessionSearchService  
    {
        Task<ISearchResponse<SessionDocument>> SearchSessionsByNotesAsync(string keywords, DateTime? startDate = null, DateTime? endDate = null, int size = 20);
        Task<ISearchResponse<SessionDocument>> AnalyzeSessionsByTypeAndDateAsync(string sessionType, DateTime startDate, DateTime endDate, int size = 50);
    }

    public interface IElasticsearchService
    {
        Task<bool> CreateIndexIfNotExistsAsync<T>(string indexName, object? mapping = null) where T : class;
        Task<bool> IndexDocumentAsync<T>(string indexName, T document, string? id = null) where T : class;
        Task<bool> BulkIndexAsync<T>(string indexName, IEnumerable<T> documents) where T : class;
        Task<ISearchResponse<T>> SearchAsync<T>(string indexName, object searchRequest) where T : class;
        Task<bool> DeleteDocumentAsync(string indexName, string id);
        Task<bool> UpdateDocumentAsync<T>(string indexName, string id, T document) where T : class;
    }
}