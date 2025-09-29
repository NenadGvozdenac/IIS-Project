using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Elasticsearch.Services;
using scouting_service.src.Elasticsearch.Models;

namespace scouting_service.src.Elasticsearch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerSearchController : ControllerBase
    {
        private readonly IPlayerSearchService _playerSearchService;
        private readonly IDataSyncService _dataSyncService;
        private readonly ILogger<PlayerSearchController> _logger;

        public PlayerSearchController(
            IPlayerSearchService playerSearchService,
            IDataSyncService dataSyncService,
            ILogger<PlayerSearchController> logger)
        {
            _playerSearchService = playerSearchService;
            _dataSyncService = dataSyncService;
            _logger = logger;
        }

        /// <summary>
        /// Pretraga igrača na osnovu različitih kriterijuma
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchPlayers([FromQuery] string searchTerm = "", [FromQuery] int size = 20)
        {
            try
            {
                var result = await _playerSearchService.SearchPlayersAsync(searchTerm, size);
                
                var response = new
                {
                    Total = result.Total,
                    Players = result.Documents.Select(doc => new
                    {
                        doc.Id,
                        doc.Name,
                        doc.Surname,
                        doc.Birthday,
                        doc.Height,
                        doc.Weight,
                        Position = doc.Position.Name,
                        Nationality = doc.Nationality.State,
                        PhysicalMetrics = doc.PhysicalMetrics.OrderByDescending(pm => pm.DateOfMeasurement).FirstOrDefault(),
                        doc.Bio,
                        doc.Notes
                    }),
                    Aggregations = new
                    {
                        Positions = result.Aggregations.Terms("positions")?.Buckets.Select(b => new { Key = b.Key, Count = b.DocCount }),
                        Nationalities = result.Aggregations.Terms("nationalities")?.Buckets.Select(b => new { Key = b.Key, Count = b.DocCount }),
                        AverageHeight = result.Aggregations.Average("avg_height")?.Value,
                        AverageWeight = result.Aggregations.Average("avg_weight")?.Value
                    }
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
        /// Analiza performansi igrača po poziciji (složeni upit 1)
        /// </summary>
        [HttpGet("analyze/position/{position}")]
        public async Task<IActionResult> AnalyzePlayerPerformanceByPosition(string position, [FromQuery] int size = 20)
        {
            try
            {
                var result = await _playerSearchService.AnalyzePlayerPerformanceByPositionAsync(position, size);
                
                var response = new
                {
                    Position = position,
                    TotalPlayers = result.Total,
                    TopPlayers = result.Documents.Select((doc, index) => new
                    {
                        Rank = index + 1,
                        doc.Id,
                        doc.Name,
                        doc.Surname,
                        doc.Height,
                        doc.Weight,
                        VerticalJump = doc.PhysicalMetrics.OrderByDescending(pm => pm.DateOfMeasurement).FirstOrDefault()?.VerticalJump,
                        Nationality = doc.Nationality.State
                    }),
                    Statistics = new
                    {
                        AverageHeight = result.Aggregations.Average("avg_height")?.Value,
                        AverageWeight = result.Aggregations.Average("avg_weight")?.Value,
                        AverageVerticalJump = result.Aggregations.Average("avg_vertical_jump")?.Value
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing player performance by position: {Position}", position);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Uporedna analiza fizičkih metrika (složeni upit 3)
        /// </summary>
        [HttpPost("analyze/physical-metrics")]
        public async Task<IActionResult> ComparePhysicalMetrics([FromBody] PhysicalMetricsFilter filter)
        {
            try
            {
                var result = await _playerSearchService.ComparePhysicalMetricsAsync(filter);
                
                var response = new
                {
                    FilterCriteria = new
                    {
                        filter.MaxFatPercentage,
                        filter.MinWingspan,
                        filter.MinVerticalJump
                    },
                    TotalPlayers = result.Total,
                    FilteredPlayers = result.Documents.Select((doc, index) => new
                    {
                        Rank = index + 1,
                        doc.Id,
                        doc.Name,
                        doc.Surname,
                        Position = doc.Position.Name,
                        LatestMetrics = doc.PhysicalMetrics.OrderByDescending(pm => pm.DateOfMeasurement).FirstOrDefault()
                    }),
                    Statistics = new
                    {
                        BenchPress = new
                        {
                            Min = result.Aggregations.Min("min_bench_press")?.Value,
                            Max = result.Aggregations.Max("max_bench_press")?.Value
                        },
                        Squat = new
                        {
                            Min = result.Aggregations.Min("min_squat")?.Value,
                            Max = result.Aggregations.Max("max_squat")?.Value
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