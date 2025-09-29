using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Elasticsearch.Models;
using scouting_service.src.Elasticsearch.Services;

namespace scouting_service.src.Elasticsearch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataSyncController : ControllerBase
    {
        private readonly IDataSyncService _dataSyncService;
        private readonly ILogger<DataSyncController> _logger;

        public DataSyncController(IDataSyncService dataSyncService, ILogger<DataSyncController> logger)
        {
            _dataSyncService = dataSyncService;
            _logger = logger;
        }

        /// <summary>
        /// Sync sample players to Elasticsearch
        /// </summary>
        [HttpPost("sync-sample-players")]
        public async Task<IActionResult> SyncSamplePlayers([FromBody] List<PlayerDocument> players)
        {
            try
            {
                var result = await _dataSyncService.SyncSamplePlayersAsync(players);
                
                if (result)
                {
                    return Ok(new { Message = $"Successfully synced {players.Count} sample players", Count = players.Count });
                }
                else
                {
                    return BadRequest("Failed to sync sample players");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing sample players");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Sync sample sessions to Elasticsearch
        /// </summary>
        [HttpPost("sync-sample-sessions")]
        public async Task<IActionResult> SyncSampleSessions([FromBody] List<SessionDocument> sessions)
        {
            try
            {
                var result = await _dataSyncService.SyncSampleSessionsAsync(sessions);
                
                if (result)
                {
                    return Ok(new { Message = $"Successfully synced {sessions.Count} sample sessions", Count = sessions.Count });
                }
                else
                {
                    return BadRequest("Failed to sync sample sessions");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing sample sessions");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}