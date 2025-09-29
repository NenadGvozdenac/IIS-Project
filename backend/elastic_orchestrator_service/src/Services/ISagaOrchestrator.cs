using elastic_orchestrator_service.src.Models;

namespace elastic_orchestrator_service.src.Services
{
    public interface ISagaOrchestrator
    {
        Task<SagaTransaction> UpdatePlayerAsync(UpdatePlayerRequest request);
        Task<SagaTransaction?> GetTransactionAsync(Guid transactionId);
        Task<IEnumerable<SagaTransaction>> GetTransactionsAsync();
    }
}