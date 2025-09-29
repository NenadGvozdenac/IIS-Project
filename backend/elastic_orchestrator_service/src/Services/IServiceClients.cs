using elastic_orchestrator_service.src.Models;

namespace elastic_orchestrator_service.src.Services
{
    public interface IScoutingServiceClient
    {
        Task<bool> UpdatePlayerAsync(Player player);
        Task<bool> CompensatePlayerUpdateAsync(Player player);
    }

    public interface IElasticsearchServiceClient
    {
        Task<bool> UpdatePlayerAsync(Player player);
        Task<bool> CompensatePlayerUpdateAsync(Player player);
    }
}