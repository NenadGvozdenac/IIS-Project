using Microsoft.AspNetCore.Mvc;
using elasticsearch_service.src.Models;
using elasticsearch_service.src.Models.DTOs;
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

        // Agregacije za izvještaje

        [HttpGet("aggregations/test-simple")]
        public async Task<IActionResult> TestSimpleHardcoded()
        {
            // Just return hardcoded data to test if endpoint routing works
            var result = new[]
            {
                new { IdPlayer = 1, PlayerFullName = "Test Player", IdSession = 1, Points = 42 },
                new { IdPlayer = 2, PlayerFullName = "Another Player", IdSession = 2, Points = 35 }
            };
            
            return Ok(result);
        }

        [HttpGet("aggregations/most-points-session")]
        public async Task<ActionResult<IEnumerable<PlayerSessionPointsDto>>> GetPlayersWithMostPointsInSession([FromQuery] int limit = 5)
        {
            try
            {
                var result = await _elasticsearchService.GetPlayersWithMostPointsInSessionAsync(limit);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting players with most points in session");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("aggregations/top-max-points-last-year")]
        public async Task<ActionResult<IEnumerable<PlayerMaxPointsDto>>> GetTop5PlayersWithMaxPointsLastYear()
        {
            try
            {
                var result = await _elasticsearchService.GetTop5PlayersWithMaxPointsLastYearAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top 5 players with max points last year");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("aggregations/playoff-minutes-by-nationality")]
        public async Task<ActionResult<IEnumerable<PlayerPlayoffMinutesDto>>> GetPlayersWithMostPlayoffMinutesByNationality([FromQuery] string nationality, [FromQuery] int limit = 5)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nationality))
                    return BadRequest("Nationality parameter is required");

                var result = await _elasticsearchService.GetPlayersWithMostPlayoffMinutesByNationalityAsync(nationality, limit);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting players with most playoff minutes for nationality {Nationality}", nationality);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("aggregations/top-sessions-by-metric")]
        public async Task<ActionResult<IEnumerable<PlayerMetricSessionDto>>> GetTopPlayerSessionsByMetric([FromQuery] string playerName, [FromQuery] string metricName, [FromQuery] int limit = 5)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(playerName))
                    return BadRequest("Player name is required");

                if (string.IsNullOrWhiteSpace(metricName))
                    return BadRequest("Metric name is required");

                // Prvo provjerimo da li je metrika kvantitativna
                var availableMetrics = await _elasticsearchService.GetAvailableQuantitativeMetricsAsync();
                var metricType = availableMetrics.FirstOrDefault(m => m.MetricName.Equals(metricName, StringComparison.OrdinalIgnoreCase));

                if (metricType == null || !metricType.MetricType.Equals("Quantitative", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest($"Metric '{metricName}' is not a quantitative metric or does not exist");
                }

                var result = await _elasticsearchService.GetTopPlayerSessionsByMetricAsync(playerName, metricName, limit);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top sessions for player {PlayerName} by metric {MetricName}", playerName, metricName);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("aggregations/available-quantitative-metrics")]
        public async Task<ActionResult<IEnumerable<MetricTypeDto>>> GetAvailableQuantitativeMetrics()
        {
            try
            {
                var result = await _elasticsearchService.GetAvailableQuantitativeMetricsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available quantitative metrics");
                return StatusCode(500, "Internal server error");
            }
        }

        // Debug endpoint to see raw data
        [HttpGet("debug/test-aggregation-simple")]
        public async Task<IActionResult> TestSimpleAggregation()
        {
            try
            {
                // Get raw sessions
                var rawSessions = await _elasticsearchService.GetSessionsAsync(0, 5);
                
                // Try to extract PTS manually (updated for new structure)
                var ptsData = rawSessions.Where(s => s.Metrics != null)
                    .SelectMany(s => s.Metrics.Where(m => m.MetricName == "PTS") // Changed from "points" to "PTS"
                        .Select(m => new 
                        {
                            SessionId = s.IdSession,
                            PlayerId = s.IdPlayer,
                            PlayerName = s.PlayerFullName,
                            Points = m.MetricValue,
                            MetricType = m.MetricType
                        }))
                    .OrderByDescending(x => int.TryParse(x.Points, out int p) ? p : 0)
                    .Take(3)
                    .ToList();

                // Also show what metrics we actually have
                var allMetrics = rawSessions.Where(s => s.Metrics != null)
                    .SelectMany(s => s.Metrics)
                    .Select(m => new { MetricName = m.MetricName, MetricType = m.MetricType })
                    .Distinct()
                    .Take(10)
                    .ToList();

                return Ok(new 
                {
                    TotalSessions = rawSessions.Count(),
                    SessionsWithMetrics = rawSessions.Count(s => s.Metrics != null && s.Metrics.Any()),
                    PtsData = ptsData,
                    SampleMetrics = allMetrics
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("debug/aggregation-test")]
        public async Task<IActionResult> TestAggregationDebug()
        {
            try
            {
                var result = await _elasticsearchService.GetPlayersWithMostPointsInSessionAsync(2);
                var debugInfo = new
                {
                    ResultCount = result?.Count() ?? 0,
                    Results = result?.Take(2)
                };
                return Ok(debugInfo);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error in aggregation test: {ex.Message}");
            }
        }

        [HttpGet("debug/raw-sessions")]
        public async Task<ActionResult> GetRawSessions([FromQuery] int take = 5)
        {
            try
            {
                var sessions = await _elasticsearchService.GetSessionsAsync(0, take);
                return Ok(sessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting raw sessions: {Message}", ex.Message);
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        // Bulk data operations

        [HttpPost("bulk/generate-dummy-data")]
        public async Task<ActionResult> GenerateDummyData()
        {
            try
            {
                var success = await _elasticsearchService.GenerateDummyDataAsync();
                if (!success)
                    return StatusCode(500, "Failed to generate dummy data");

                return Ok(new { message = "Dummy data generated successfully", playersCount = 100, sessionsCount = 1000 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating dummy data");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("bulk/players")]
        public async Task<ActionResult> BulkInsertPlayers([FromBody] IEnumerable<Player> players)
        {
            try
            {
                if (players == null || !players.Any())
                    return BadRequest("Players data is required");

                var success = await _elasticsearchService.BulkInsertPlayersAsync(players);
                if (!success)
                    return StatusCode(500, "Failed to bulk insert players");

                return Ok(new { message = "Players inserted successfully", count = players.Count() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk inserting players");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}