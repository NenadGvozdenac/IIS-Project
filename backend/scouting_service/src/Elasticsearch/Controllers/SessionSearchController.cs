using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Elasticsearch.Services;
using scouting_service.src.Elasticsearch.Models;

namespace scouting_service.src.Elasticsearch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionSearchController : ControllerBase
    {
        private readonly ISessionSearchService _sessionSearchService;
        private readonly IDataSyncService _dataSyncService;
        private readonly ILogger<SessionSearchController> _logger;

        public SessionSearchController(
            ISessionSearchService sessionSearchService,
            IDataSyncService dataSyncService,
            ILogger<SessionSearchController> logger)
        {
            _sessionSearchService = sessionSearchService;
            _dataSyncService = dataSyncService;
            _logger = logger;
        }

        /// <summary>
        /// Pretraga sesija na osnovu različitih kriterijuma
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchSessions([FromQuery] string keywords = "", [FromQuery] int size = 20)
        {
            try
            {
                var result = await _sessionSearchService.SearchSessionsByNotesAsync(keywords, null, null, size);
                
                var response = new
                {
                    Total = result.Total,
                    Sessions = result.Documents.Select(doc => new
                    {
                        doc.Id,
                        doc.StartTime,
                        doc.EndTime,
                        doc.DurationMinutes,
                        SessionType = doc.SessionType.Type,
                        SessionStatus = doc.SessionStatus.Status,
                        User = new { doc.User.Name, doc.User.Surname },
                        ParticipantsCount = doc.Participants.Count,
                        doc.Location,
                        doc.Tags,
                        doc.Notes,
                        doc.Description
                    }),
                    Aggregations = new
                    {
                        SessionTypes = result.Aggregations.Terms("session_types")?.Buckets.Select(b => new { Key = b.Key, Count = b.DocCount }),
                        SessionStatuses = result.Aggregations.Terms("session_statuses")?.Buckets.Select(b => new { Key = b.Key, Count = b.DocCount }),
                        Users = result.Aggregations.Terms("users")?.Buckets.Select(b => new { Key = b.Key, Count = b.DocCount }),
                        AverageDuration = result.Aggregations.Average("avg_duration")?.Value,
                        SessionsByMonth = result.Aggregations.DateHistogram("sessions_by_month")?.Buckets.Select(b => new 
                        { 
                            Month = b.KeyAsString, 
                            Count = b.DocCount 
                        })
                    }
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
        /// Pretraga i analiza sesija po tipu i datumu (složeni upit 2)
        /// </summary>
        [HttpGet("analyze/type/{sessionType}")]
        public async Task<IActionResult> AnalyzeSessionsByTypeAndDate(
            string sessionType,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var start = startDate ?? DateTime.Now.AddMonths(-6);
                var end = endDate ?? DateTime.Now;

                var result = await _sessionSearchService.AnalyzeSessionsByTypeAndDateAsync(sessionType, start, end);
                
                var response = new
                {
                    SessionType = sessionType,
                    DateRange = new { StartDate = start, EndDate = end },
                    TotalSessions = result.Total,
                    Sessions = result.Documents.Select(doc => new
                    {
                        doc.Id,
                        doc.StartTime,
                        doc.EndTime,
                        doc.DurationMinutes,
                        Status = doc.SessionStatus.Status,
                        User = new { doc.User.Name, doc.User.Surname },
                        ParticipantsCount = doc.Participants.Count,
                        doc.Location
                    }),
                    Analytics = new
                    {
                        SessionsOverTime = result.Aggregations.DateHistogram("sessions_over_time")?.Buckets.Select(b => new 
                        { 
                            Month = b.KeyAsString, 
                            Count = b.DocCount 
                        }),
                        SessionsByUser = result.Aggregations.Terms("by_user")?.Buckets.Select(b => new 
                        { 
                            User = b.Key, 
                            SessionCount = b.DocCount 
                        }),
                        AverageDuration = result.Aggregations.Average("avg_duration")?.Value,
                        SessionsByStatus = result.Aggregations.Terms("by_status")?.Buckets.Select(b => new 
                        { 
                            Status = b.Key, 
                            Count = b.DocCount 
                        })
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing sessions by type and date: {SessionType}", sessionType);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Pretraga sesija po napomenama sa tekstualnom pretragom
        /// </summary>
        [HttpGet("search/notes")]
        public async Task<IActionResult> SearchSessionsByNotes(
            [FromQuery] string searchText,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                if (string.IsNullOrEmpty(searchText))
                {
                    return BadRequest("Search text is required");
                }

                var result = await _sessionSearchService.SearchSessionsByNotesAsync(searchText, startDate, endDate);
                
                var response = new
                {
                    SearchText = searchText,
                    DateRange = startDate.HasValue || endDate.HasValue ? new { StartDate = startDate, EndDate = endDate } : null,
                    Total = result.Total,
                    Sessions = result.Documents.Select(doc => new
                    {
                        doc.Id,
                        doc.StartTime,
                        doc.EndTime,
                        SessionType = doc.SessionType.Type,
                        Status = doc.SessionStatus.Status,
                        User = new { doc.User.Name, doc.User.Surname },
                        doc.Notes,
                        doc.Description,
                        doc.Location,
                        // Dodaj highlight informacije ako su dostupne
                        Highlights = result.Hits.FirstOrDefault(h => h.Source.Id == doc.Id)?.Highlight
                    })
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching sessions by notes with text: {SearchText}", searchText);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}