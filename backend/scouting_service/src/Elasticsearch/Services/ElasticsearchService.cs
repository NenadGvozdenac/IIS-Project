using Nest;

namespace scouting_service.src.Elasticsearch.Services
{
    public class ElasticsearchService : IElasticsearchService
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ElasticsearchService> _logger;

        public ElasticsearchService(IElasticClient elasticClient, ILogger<ElasticsearchService> logger)
        {
            _elasticClient = elasticClient;
            _logger = logger;
        }

        public async Task<bool> CreateIndexIfNotExistsAsync<T>(string indexName, object? mapping = null) where T : class
        {
            try
            {
                var existsResponse = await _elasticClient.Indices.ExistsAsync(indexName);
                if (existsResponse.Exists)
                {
                    return true;
                }

                var createIndexResponse = await _elasticClient.Indices.CreateAsync(indexName, c => c
                    .Map<T>(m => m.AutoMap())
                    .Settings(s => s
                        .NumberOfShards(1)
                        .NumberOfReplicas(0)
                    )
                );

                if (!createIndexResponse.IsValid)
                {
                    _logger.LogError("Failed to create index {IndexName}: {Error}", indexName, createIndexResponse.DebugInformation);
                    return false;
                }

                _logger.LogInformation("Successfully created index {IndexName}", indexName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while creating index {IndexName}", indexName);
                return false;
            }
        }

        public async Task<bool> IndexDocumentAsync<T>(string indexName, T document, string? id = null) where T : class
        {
            try
            {
                IndexResponse response;
                if (!string.IsNullOrEmpty(id))
                {
                    response = await _elasticClient.IndexAsync(document, i => i.Index(indexName).Id(id));
                }
                else
                {
                    response = await _elasticClient.IndexAsync(document, i => i.Index(indexName));
                }

                if (!response.IsValid)
                {
                    _logger.LogError("Failed to index document in {IndexName}: {Error}", indexName, response.DebugInformation);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while indexing document in {IndexName}", indexName);
                return false;
            }
        }

        public async Task<ISearchResponse<T>> SearchAsync<T>(string indexName, object searchRequest) where T : class
        {
            try
            {
                // Za jednostavnost, koristim osnovni search
                var response = await _elasticClient.SearchAsync<T>(s => s
                    .Index(indexName)
                    .Size(20)
                );

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while searching in {IndexName}", indexName);
                throw;
            }
        }

        public async Task<bool> DeleteDocumentAsync(string indexName, string id)
        {
            try
            {
                var response = await _elasticClient.DeleteAsync<object>(id, d => d.Index(indexName));
                
                if (!response.IsValid)
                {
                    _logger.LogError("Failed to delete document {Id} from {IndexName}: {Error}", id, indexName, response.DebugInformation);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while deleting document {Id} from {IndexName}", id, indexName);
                return false;
            }
        }

        public async Task<bool> UpdateDocumentAsync<T>(string indexName, string id, T document) where T : class
        {
            try
            {
                var response = await _elasticClient.UpdateAsync<T>(id, u => u
                    .Index(indexName)
                    .Doc(document)
                );

                if (!response.IsValid)
                {
                    _logger.LogError("Failed to update document {Id} in {IndexName}: {Error}", id, indexName, response.DebugInformation);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while updating document {Id} in {IndexName}", id, indexName);
                return false;
            }
        }

        public async Task<bool> BulkIndexAsync<T>(string indexName, IEnumerable<T> documents) where T : class
        {
            try
            {
                var bulkResponse = await _elasticClient.BulkAsync(b => b
                    .Index(indexName)
                    .IndexMany(documents)
                );

                if (!bulkResponse.IsValid)
                {
                    _logger.LogError("Bulk indexing failed: {Error}", bulkResponse.DebugInformation);
                    return false;
                }

                _logger.LogInformation("Successfully bulk indexed {Count} documents to {IndexName}", documents.Count(), indexName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk indexing to {IndexName}", indexName);
                return false;
            }
        }
    }
}