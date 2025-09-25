using Microsoft.AspNetCore.Mvc;
using elasticsearch_service.src.Models;
using elasticsearch_service.src.Services;

namespace elasticsearch_service.src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly IElasticsearchService _elasticsearchService;
        private readonly ILogger<PlayersController> _logger;

        public PlayersController(IElasticsearchService elasticsearchService, ILogger<PlayersController> logger)
        {
            _elasticsearchService = elasticsearchService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayers([FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            try
            {
                var players = await _elasticsearchService.GetPlayersAsync(skip, take);
                return Ok(players);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting players");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Player>> GetPlayer(int id)
        {
            try
            {
                var player = await _elasticsearchService.GetPlayerAsync(id);
                if (player == null)
                    return NotFound($"Player with id {id} not found");

                return Ok(player);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting player {PlayerId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Player>> CreatePlayer([FromBody] Player player)
        {
            try
            {
                if (player == null)
                    return BadRequest("Player data is required");

                var success = await _elasticsearchService.CreatePlayerAsync(player);
                if (!success)
                    return StatusCode(500, "Failed to create player");

                return CreatedAtAction(nameof(GetPlayer), new { id = player.IdPlayer }, player);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating player");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePlayer(int id, [FromBody] Player player)
        {
            try
            {
                if (player == null)
                    return BadRequest("Player data is required");

                if (id != player.IdPlayer)
                    return BadRequest("Player ID mismatch");

                var success = await _elasticsearchService.UpdatePlayerAsync(player);
                if (!success)
                    return StatusCode(500, "Failed to update player");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating player {PlayerId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePlayer(int id)
        {
            try
            {
                var success = await _elasticsearchService.DeletePlayerAsync(id);
                if (!success)
                    return NotFound($"Player with id {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting player {PlayerId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Player>>> SearchPlayers([FromQuery] string searchTerm, [FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequest("Search term is required");

                var players = await _elasticsearchService.SearchPlayersAsync(searchTerm, skip, take);
                return Ok(players);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching players with term {SearchTerm}", searchTerm);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}