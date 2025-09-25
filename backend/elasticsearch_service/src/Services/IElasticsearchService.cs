using elasticsearch_service.src.Models;
using elasticsearch_service.src.Models.DTOs;

namespace elasticsearch_service.src.Services
{
    public interface IElasticsearchService
    {
        // Players
        Task<bool> CreatePlayerAsync(Player player);
        Task<Player?> GetPlayerAsync(int id);
        Task<IEnumerable<Player>> GetPlayersAsync(int skip = 0, int take = 10);
        Task<bool> UpdatePlayerAsync(Player player);
        Task<bool> DeletePlayerAsync(int id);
        Task<IEnumerable<Player>> SearchPlayersAsync(string searchTerm, int skip = 0, int take = 10);

        // Sessions
        Task<bool> CreateSessionAsync(Session session);
        Task<Session?> GetSessionAsync(int id);
        Task<IEnumerable<Session>> GetSessionsAsync(int skip = 0, int take = 10);
        Task<bool> UpdateSessionAsync(Session session);
        Task<bool> DeleteSessionAsync(int id);
        Task<IEnumerable<Session>> SearchSessionsAsync(string searchTerm, int skip = 0, int take = 10);
        Task<IEnumerable<Session>> GetSessionsByPlayerAsync(int playerId, int skip = 0, int take = 10);

        // Index management
        Task<bool> CreateIndexesAsync();
        Task<bool> DeleteIndexesAsync();

        // Aggregation methods
        Task<IEnumerable<PlayerSessionPointsDto>> GetPlayersWithMostPointsInSessionAsync(int limit = 5);
        Task<IEnumerable<PlayerMaxPointsDto>> GetTop5PlayersWithMaxPointsLastYearAsync();
        Task<IEnumerable<PlayerPlayoffMinutesDto>> GetPlayersWithMostPlayoffMinutesByNationalityAsync(string nationality, int limit = 5);
        Task<IEnumerable<PlayerMetricSessionDto>> GetTopPlayerSessionsByMetricAsync(string playerName, string metricName, int limit = 5);
        Task<IEnumerable<MetricTypeDto>> GetAvailableQuantitativeMetricsAsync();

        // Bulk data operations
        Task<bool> BulkInsertPlayersAsync(IEnumerable<Player> players);
        Task<bool> BulkInsertSessionsAsync(IEnumerable<Session> sessions);
        Task<bool> GenerateDummyDataAsync();
    }
}