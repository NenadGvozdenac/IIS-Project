using Microsoft.AspNetCore.Mvc;
using elasticsearch_service.src.Models;
using elasticsearch_service.src.Services;

namespace elasticsearch_service.src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionsController : ControllerBase
    {
        private readonly IElasticsearchService _elasticsearchService;
        private readonly ILogger<SessionsController> _logger;

        public SessionsController(IElasticsearchService elasticsearchService, ILogger<SessionsController> logger)
        {
            _elasticsearchService = elasticsearchService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Session>>> GetSessions([FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            try
            {
                var sessions = await _elasticsearchService.GetSessionsAsync(skip, take);
                return Ok(sessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sessions");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Session>> GetSession(int id)
        {
            try
            {
                var session = await _elasticsearchService.GetSessionAsync(id);
                if (session == null)
                    return NotFound($"Session with id {id} not found");

                return Ok(session);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting session {SessionId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Session>> CreateSession([FromBody] Session session)
        {
            try
            {
                if (session == null)
                    return BadRequest("Session data is required");

                var success = await _elasticsearchService.CreateSessionAsync(session);
                if (!success)
                    return StatusCode(500, "Failed to create session");

                return CreatedAtAction(nameof(GetSession), new { id = session.IdSession }, session);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating session");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateSession(int id, [FromBody] Session session)
        {
            try
            {
                if (session == null)
                    return BadRequest("Session data is required");

                if (id != session.IdSession)
                    return BadRequest("Session ID mismatch");

                var success = await _elasticsearchService.UpdateSessionAsync(session);
                if (!success)
                    return StatusCode(500, "Failed to update session");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating session {SessionId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSession(int id)
        {
            try
            {
                var success = await _elasticsearchService.DeleteSessionAsync(id);
                if (!success)
                    return NotFound($"Session with id {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting session {SessionId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Session>>> SearchSessions([FromQuery] string searchTerm, [FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequest("Search term is required");

                var sessions = await _elasticsearchService.SearchSessionsAsync(searchTerm, skip, take);
                return Ok(sessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching sessions with term {SearchTerm}", searchTerm);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("player/{playerId}")]
        public async Task<ActionResult<IEnumerable<Session>>> GetSessionsByPlayer(int playerId, [FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            try
            {
                var sessions = await _elasticsearchService.GetSessionsByPlayerAsync(playerId, skip, take);
                return Ok(sessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sessions for player {PlayerId}", playerId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("bulk")]
        public async Task<ActionResult> BulkInsertSessions([FromBody] IEnumerable<Session> sessions)
        {
            try
            {
                if (sessions == null || !sessions.Any())
                    return BadRequest("Sessions data is required");

                var success = await _elasticsearchService.BulkInsertSessionsAsync(sessions);
                if (!success)
                    return StatusCode(500, "Failed to bulk insert sessions");

                return Ok(new { message = "Sessions inserted successfully", count = sessions.Count() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk inserting sessions");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}