using Microsoft.AspNetCore.Mvc;
using Nest;
using scouting_service.src.Elasticsearch.Services;
using scouting_service.src.Elasticsearch.Models;

namespace scouting_service.src.Elasticsearch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IPlayerSearchService _playerSearchService;
        private readonly ISessionSearchService _sessionSearchService;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(
            IPlayerSearchService playerSearchService,
            ISessionSearchService sessionSearchService,
            ILogger<AnalyticsController> logger)
        {
            _playerSearchService = playerSearchService;
            _sessionSearchService = sessionSearchService;
            _logger = logger;
        }

        /// <summary>
        /// Jednostavan upit - Pretraživanje igrača
        /// </summary>
        [HttpGet("search-players")]
        public async Task<IActionResult> SearchPlayers([FromQuery] string searchTerm = "", [FromQuery] int size = 20)
        {
            try
            {
                var result = await _playerSearchService.SearchPlayersAsync(searchTerm, size);
                
                var response = new
                {
                    TotalResults = result.Total,
                    Players = result.Documents.Select(player => new
                    {
                        player.Id,
                        Name = $"{player.Name} {player.Surname}",
                        player.Height,
                        player.Weight,
                        Position = player.Position.Name,
                        Nationality = player.Nationality.State,
                        LatestMetrics = player.PhysicalMetrics
                            .OrderByDescending(pm => pm.DateOfMeasurement)
                            .FirstOrDefault()
                    })
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching players");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Analiza performansi igrača po poziciji
        /// </summary>
        [HttpGet("player-performance/{position}")]
        public async Task<IActionResult> AnalyzePlayerPerformance(string position, [FromQuery] int size = 20)
        {
            try
            {
                var result = await _playerSearchService.AnalyzePlayerPerformanceByPositionAsync(position, size);
                
                var response = new
                {
                    Position = position,
                    TotalPlayers = result.Total,
                    Players = result.Documents.Select((player, index) => new
                    {
                        Rank = index + 1,
                        player.Id,
                        Name = $"{player.Name} {player.Surname}",
                        player.Height,
                        player.Weight,
                        Nationality = player.Nationality.State,
                        LatestMetrics = player.PhysicalMetrics
                            .OrderByDescending(pm => pm.DateOfMeasurement)
                            .FirstOrDefault()
                    }),
                    Statistics = new
                    {
                        AverageHeight = "Aggregations available in complex queries",
                        AverageWeight = "Use /api/analytics/compare-physical-metrics for detailed stats"
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing player performance");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Pretraživanje sesija po napomenama
        /// </summary>
        [HttpGet("search-sessions")]
        public async Task<IActionResult> SearchSessions(
            [FromQuery] string keywords = "",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int size = 20)
        {
            try
            {
                var result = await _sessionSearchService.SearchSessionsByNotesAsync(keywords, startDate, endDate, size);
                
                var response = new
                {
                    TotalResults = result.Total,
                    SearchPeriod = new
                    {
                        StartDate = startDate,
                        EndDate = endDate
                    },
                    Sessions = result.Documents.Select(session => new
                    {
                        session.Id,
                        session.Description,
                        session.StartTime,
                        session.DurationMinutes,
                        SessionType = session.SessionType,
                        Location = session.Location,
                        Trainer = $"{session.User.Name} {session.User.Surname}",
                        ParticipantCount = session.Participants.Count,
                        session.Notes
                    })
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching sessions");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Uporedna analiza fizičkih metrika
        /// </summary>
        [HttpPost("compare-physical-metrics")]
        public async Task<IActionResult> ComparePhysicalMetrics([FromBody] PhysicalMetricsFilter filter)
        {
            try
            {
                var result = await _playerSearchService.ComparePhysicalMetricsAsync(filter);
                
                var response = new
                {
                    Criteria = filter,
                    TotalPlayers = result.Total,
                    Players = result.Documents.Select(player => new
                    {
                        player.Id,
                        Name = $"{player.Name} {player.Surname}",
                        Position = player.Position.Name,
                        Nationality = player.Nationality.State,
                        player.Height,
                        player.Weight,
                        LatestMetrics = player.PhysicalMetrics
                            .OrderByDescending(pm => pm.DateOfMeasurement)
                            .FirstOrDefault()
                    }),
                    Statistics = new
                    {
                        Message = "Aggragacije su dostupne kroz detaljne upite",
                        FilterApplied = new
                        {
                            filter.MinVerticalJump,
                            filter.MaxFatPercentage,
                            filter.MinWingspan
                        }
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comparing physical metrics");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}