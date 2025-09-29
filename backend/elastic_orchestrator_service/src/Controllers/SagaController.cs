using Microsoft.AspNetCore.Mvc;
using elastic_orchestrator_service.src.Models;
using elastic_orchestrator_service.src.Services;

namespace elastic_orchestrator_service.src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SagaController : ControllerBase
    {
        private readonly ISagaOrchestrator _sagaOrchestrator;
        private readonly ILogger<SagaController> _logger;

        public SagaController(ISagaOrchestrator sagaOrchestrator, ILogger<SagaController> logger)
        {
            _sagaOrchestrator = sagaOrchestrator;
            _logger = logger;
        }

        [HttpPost("update-player")]
        public async Task<ActionResult<SagaTransaction>> UpdatePlayer([FromBody] UpdatePlayerRequest request)
        {
            try
            {
                if (request?.Player == null)
                    return BadRequest("Player data is required");

                if (request.Player.IdPlayer <= 0)
                    return BadRequest("Valid Player ID is required");

                _logger.LogInformation("Starting saga for updating player {PlayerId}", request.Player.IdPlayer);

                var transaction = await _sagaOrchestrator.UpdatePlayerAsync(request);
                
                return Accepted(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting saga for player update");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("transactions/{transactionId}")]
        public async Task<ActionResult<SagaTransaction>> GetTransaction(Guid transactionId)
        {
            try
            {
                var transaction = await _sagaOrchestrator.GetTransactionAsync(transactionId);
                
                if (transaction == null)
                    return NotFound($"Transaction {transactionId} not found");

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transaction {TransactionId}", transactionId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("transactions")]
        public async Task<ActionResult<IEnumerable<SagaTransaction>>> GetTransactions()
        {
            try
            {
                var transactions = await _sagaOrchestrator.GetTransactionsAsync();
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transactions");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}