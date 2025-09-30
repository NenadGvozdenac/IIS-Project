using elastic_orchestrator_service.src.Models;
using System.Collections.Concurrent;
using System.Text.Json;

namespace elastic_orchestrator_service.src.Services
{
    public class SagaOrchestrator : ISagaOrchestrator
    {
        private readonly IScoutingServiceClient _scoutingServiceClient;
        private readonly IElasticsearchServiceClient _elasticsearchServiceClient;
        private readonly ILogger<SagaOrchestrator> _logger;
        private readonly ConcurrentDictionary<Guid, SagaTransaction> _transactions;

        public SagaOrchestrator(
            IScoutingServiceClient scoutingServiceClient,
            IElasticsearchServiceClient elasticsearchServiceClient,
            ILogger<SagaOrchestrator> logger)
        {
            _scoutingServiceClient = scoutingServiceClient;
            _elasticsearchServiceClient = elasticsearchServiceClient;
            _logger = logger;
            _transactions = new ConcurrentDictionary<Guid, SagaTransaction>();
        }

        public Task<SagaTransaction> UpdatePlayerAsync(UpdatePlayerRequest request)
        {
            var transactionId = Guid.NewGuid();
            var transaction = new SagaTransaction
            {
                TransactionId = transactionId,
                Status = SagaStatuses.Started,
                Player = request.Player,
                CreatedAt = DateTime.UtcNow,
                Steps = new List<SagaStep>
                {
                    new SagaStep { StepName = "UpdateScoutingService", Status = StepStatuses.Pending },
                    new SagaStep { StepName = "UpdateElasticsearchService", Status = StepStatuses.Pending }
                }
            };

            _transactions.TryAdd(transactionId, transaction);
            _logger.LogInformation("Started saga transaction {TransactionId} for player {PlayerId}", 
                transactionId, request.Player.IdPlayer);

            // Execute saga steps asynchronously
            _ = Task.Run(() => ExecuteSagaAsync(transaction));

            return Task.FromResult(transaction);
        }

        public Task<SagaTransaction?> GetTransactionAsync(Guid transactionId)
        {
            _transactions.TryGetValue(transactionId, out var transaction);
            return Task.FromResult(transaction);
        }

        public Task<IEnumerable<SagaTransaction>> GetTransactionsAsync()
        {
            return Task.FromResult(_transactions.Values.ToList().AsEnumerable());
        }

        private async Task ExecuteSagaAsync(SagaTransaction transaction)
        {
            try
            {
                transaction.Status = SagaStatuses.InProgress;
                _logger.LogInformation("Executing saga transaction {TransactionId}", transaction.TransactionId);

                // Get original states before making any changes
                await StoreOriginalStatesAsync(transaction);

                // Step 1: Update Scouting Service
                var scoutingStep = transaction.Steps.First(s => s.StepName == "UpdateScoutingService");
                var scoutingSuccess = await ExecuteStepAsync(scoutingStep, () => 
                    _scoutingServiceClient.UpdatePlayerAsync(transaction.Player));

                if (!scoutingSuccess)
                {
                    await CompensateTransaction(transaction);
                    return;
                }

                // Step 2: Update Elasticsearch Service
                var elasticsearchStep = transaction.Steps.First(s => s.StepName == "UpdateElasticsearchService");
                var elasticsearchSuccess = await ExecuteStepAsync(elasticsearchStep, () => 
                    _elasticsearchServiceClient.UpdatePlayerAsync(transaction.Player));

                if (!elasticsearchSuccess)
                {
                    await CompensateTransaction(transaction);
                    return;
                }

                // All steps completed successfully
                transaction.Status = SagaStatuses.Completed;
                transaction.CompletedAt = DateTime.UtcNow;
                _logger.LogInformation("Saga transaction {TransactionId} completed successfully", transaction.TransactionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing saga transaction {TransactionId}", transaction.TransactionId);
                transaction.Status = SagaStatuses.Failed;
                transaction.ErrorMessage = ex.Message;
            }
        }

        private async Task StoreOriginalStatesAsync(SagaTransaction transaction)
        {
            try
            {
                _logger.LogInformation("Storing original states for transaction {TransactionId}", transaction.TransactionId);

                // Get original state from scouting service
                var originalScoutingPlayer = await _scoutingServiceClient.GetPlayerAsync(transaction.Player.IdPlayer);
                if (originalScoutingPlayer != null)
                {
                    var scoutingJson = JsonSerializer.Serialize(originalScoutingPlayer, new JsonSerializerOptions 
                    { 
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                    });
                    transaction.OriginalStates["ScoutingService"] = scoutingJson;
                    _logger.LogDebug("Stored original scouting service state for player {PlayerId}", transaction.Player.IdPlayer);
                }

                // Get original state from elasticsearch service
                var originalElasticsearchPlayer = await _elasticsearchServiceClient.GetPlayerAsync(transaction.Player.IdPlayer);
                if (originalElasticsearchPlayer != null)
                {
                    var elasticsearchJson = JsonSerializer.Serialize(originalElasticsearchPlayer, new JsonSerializerOptions 
                    { 
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                    });
                    transaction.OriginalStates["ElasticsearchService"] = elasticsearchJson;
                    _logger.LogDebug("Stored original elasticsearch service state for player {PlayerId}", transaction.Player.IdPlayer);
                }

                _logger.LogInformation("Successfully stored original states for transaction {TransactionId}", transaction.TransactionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing original states for transaction {TransactionId}", transaction.TransactionId);
                // Don't throw here - we still want to proceed with the transaction
                // The compensation might not work perfectly, but the transaction can continue
            }
        }

        private async Task<bool> ExecuteStepAsync(SagaStep step, Func<Task<bool>> stepAction)
        {
            try
            {
                step.Status = StepStatuses.InProgress;
                step.ExecutedAt = DateTime.UtcNow;

                var result = await stepAction();

                if (result)
                {
                    step.Status = StepStatuses.Completed;
                    _logger.LogInformation("Step {StepName} completed successfully", step.StepName);
                    return true;
                }
                else
                {
                    step.Status = StepStatuses.Failed;
                    step.ErrorMessage = "Step execution returned false";
                    _logger.LogWarning("Step {StepName} failed", step.StepName);
                    return false;
                }
            }
            catch (Exception ex)
            {
                step.Status = StepStatuses.Failed;
                step.ErrorMessage = ex.Message;
                _logger.LogError(ex, "Error executing step {StepName}", step.StepName);
                return false;
            }
        }

        private async Task CompensateTransaction(SagaTransaction transaction)
        {
            try
            {
                transaction.Status = SagaStatuses.Compensating;
                _logger.LogInformation("Starting compensation for transaction {TransactionId}", transaction.TransactionId);

                // Compensate in reverse order
                var completedSteps = transaction.Steps
                    .Where(s => s.Status == StepStatuses.Completed)
                    .Reverse()
                    .ToList();

                foreach (var step in completedSteps)
                {
                    await CompensateStepAsync(step, transaction.Player, transaction);
                }

                transaction.Status = SagaStatuses.Compensated;
                transaction.CompletedAt = DateTime.UtcNow;
                _logger.LogInformation("Transaction {TransactionId} compensated successfully", transaction.TransactionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error compensating transaction {TransactionId}", transaction.TransactionId);
                transaction.Status = SagaStatuses.Failed;
                transaction.ErrorMessage = $"Compensation failed: {ex.Message}";
            }
        }

        private async Task CompensateStepAsync(SagaStep step, Player player, SagaTransaction transaction)
        {
            try
            {
                _logger.LogInformation("Compensating step {StepName}", step.StepName);

                bool compensationSuccess = false;

                switch (step.StepName)
                {
                    case "UpdateScoutingService":
                        if (transaction.OriginalStates.TryGetValue("ScoutingService", out var scoutingJson))
                        {
                            var originalPlayer = JsonSerializer.Deserialize<Player>(scoutingJson, new JsonSerializerOptions 
                            { 
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                PropertyNameCaseInsensitive = true
                            });
                            if (originalPlayer != null)
                            {
                                compensationSuccess = await _scoutingServiceClient.CompensatePlayerUpdateAsync(originalPlayer);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("No original scouting service state found for compensation");
                        }
                        break;
                    case "UpdateElasticsearchService":
                        if (transaction.OriginalStates.TryGetValue("ElasticsearchService", out var elasticsearchJson))
                        {
                            var originalPlayer = JsonSerializer.Deserialize<Player>(elasticsearchJson, new JsonSerializerOptions 
                            { 
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                PropertyNameCaseInsensitive = true
                            });
                            if (originalPlayer != null)
                            {
                                compensationSuccess = await _elasticsearchServiceClient.CompensatePlayerUpdateAsync(originalPlayer);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("No original elasticsearch service state found for compensation");
                        }
                        break;
                    default:
                        _logger.LogWarning("Unknown step name for compensation: {StepName}", step.StepName);
                        break;
                }

                if (compensationSuccess)
                {
                    step.IsCompensated = true;
                    step.CompensatedAt = DateTime.UtcNow;
                    _logger.LogInformation("Step {StepName} compensated successfully", step.StepName);
                }
                else
                {
                    _logger.LogWarning("Failed to compensate step {StepName}", step.StepName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error compensating step {StepName}", step.StepName);
            }
        }
    }
}