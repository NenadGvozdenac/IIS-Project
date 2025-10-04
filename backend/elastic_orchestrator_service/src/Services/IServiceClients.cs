using elastic_orchestrator_service.src.Models;

namespace elastic_orchestrator_service.src.Services
{
    public interface IScoutingServiceClient
    {
        Task<Player?> GetPlayerAsync(int playerId);
        Task<bool> UpdatePlayerAsync(Player player);
        Task<bool> CompensatePlayerUpdateAsync(Player originalPlayer);
    }

    public interface IElasticsearchServiceClient
    {
        Task<Player?> GetPlayerAsync(int playerId);
        Task<bool> UpdatePlayerAsync(Player player);
        Task<bool> CompensatePlayerUpdateAsync(Player originalPlayer);
    }
}