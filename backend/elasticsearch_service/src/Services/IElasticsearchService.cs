using elasticsearch_service.src.Models;

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
    }
}