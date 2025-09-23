using scouting_service.src.Elasticsearch.Models;

namespace scouting_service.src.Elasticsearch.Services
{
    public interface IDataSyncService
    {
        Task<bool> SyncSamplePlayersAsync(List<PlayerDocument> players);
        Task<bool> SyncSampleSessionsAsync(List<SessionDocument> sessions);
    }

    public class DataSyncService : IDataSyncService
    {
        private readonly IElasticsearchService _elasticsearchService;
        private readonly ILogger<DataSyncService> _logger;

        public DataSyncService(IElasticsearchService elasticsearchService, ILogger<DataSyncService> logger)
        {
            _elasticsearchService = elasticsearchService;
            _logger = logger;
        }

        public async Task<bool> SyncSamplePlayersAsync(List<PlayerDocument> players)
        {
            try
            {
                foreach (var player in players)
                {
                    await _elasticsearchService.IndexDocumentAsync("players", player, player.Id.ToString());
                }
                
                _logger.LogInformation("Successfully synced {Count} sample players", players.Count);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing sample players");
                return false;
            }
        }

        public async Task<bool> SyncSampleSessionsAsync(List<SessionDocument> sessions)
        {
            try
            {
                foreach (var session in sessions)
                {
                    await _elasticsearchService.IndexDocumentAsync("sessions", session, session.Id.ToString());
                }
                
                _logger.LogInformation("Successfully synced {Count} sample sessions", sessions.Count);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing sample sessions");
                return false;
            }
        }
    }
}