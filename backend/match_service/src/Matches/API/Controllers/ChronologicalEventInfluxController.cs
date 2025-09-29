using match_service.src.Matches.Core.Application.Features.ChronologicalEvent.CreateEventInflux;
using match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetMatchEventsInflux;
using match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetAdvancedMatchStatistics;
using match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetPlayerPerformanceComparison;
using match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetSeasonPlayerAverages;
using match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetMatchScoringEventsInfluxReport;
using match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetPlayerEventCountsInfluxReport;
using match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetTeamPlayerAveragesInfluxReport;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChronologicalEventInfluxController : BaseController
    {
        private readonly IMediator _mediator;

        public ChronologicalEventInfluxController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new event in InfluxDB timeseries database
        /// </summary>
        /// <param name="command">Event data including match, team, player info and basketball metrics</param>
        /// <returns>Success status with timestamp and event details</returns>
        [HttpPost]
        public async Task<ActionResult> CreateEventInflux([FromBody] CreateEventInfluxCommand command)
        {
            var result = await _mediator.Send(command);
            return CreateResponse(result);
        }

        /// <summary>
        /// Gets events for a specific match from InfluxDB with optional filtering
        /// </summary>
        /// <param name="matchId">Match identifier</param>
        /// <param name="eventCategory">Optional: Filter by event category (personal, team, general)</param>
        /// <param name="startTime">Optional: Filter events after this time</param>
        /// <param name="endTime">Optional: Filter events before this time</param>
        /// <param name="period">Optional: Filter by period (Q1, Q2, Q3, Q4, OT)</param>
        /// <returns>List of events with analytics data</returns>
        [HttpGet("match/{matchId}")]
        public async Task<ActionResult> GetMatchEventsInflux(
            string matchId,
            [FromQuery] string? eventCategory = null,
            [FromQuery] DateTime? startTime = null,
            [FromQuery] DateTime? endTime = null,
            [FromQuery] string? period = null)
        {
            var query = new GetMatchEventsInfluxQuery
            {
                MatchId = matchId,
                EventCategory = eventCategory,
                StartTime = startTime,
                EndTime = endTime,
                Period = period
            };

            var result = await _mediator.Send(query);
            return CreateResponse(result);
        }

        /// <summary>
        /// Gets event type counts for match analytics
        /// </summary>
        /// <param name="matchId">Match identifier</param>
        /// <returns>Dictionary of event types and their counts</returns>
        [HttpGet("match/{matchId}/analytics")]
        public async Task<ActionResult> GetMatchAnalytics(string matchId)
        {
            // This uses the existing GetMatchEventsInflux which includes EventTypeCounts
            var query = new GetMatchEventsInfluxQuery { MatchId = matchId };
            var result = await _mediator.Send(query);
            
            if (result.IsSuccess)
            {
                // Return just the analytics part
                return Ok(new
                {
                    MatchId = matchId,
                    EventTypeCounts = result.Value.EventTypeCounts,
                    TotalEvents = result.Value.TotalCount
                });
            }

            return CreateResponse(result);
        }

        /// <summary>
        /// SLOŽEN UPIT 1: Analiza efikasnosti igrača po periodima
        /// Kombinuje filtriranje, grupisanje, agregaciju i sortiranje
        /// </summary>
        [HttpGet("match/{matchId}/advanced-statistics")]
        public async Task<ActionResult> GetAdvancedMatchStatistics(string matchId)
        {
            var query = new GetAdvancedMatchStatisticsQuery { MatchId = matchId };
            var result = await _mediator.Send(query);
            return CreateResponse(result);
        }

        /// <summary>
        /// SLOŽEN UPIT 2: Poređenje performansi igrača
        /// Rangiranje sa agregatnim funkcijama
        /// </summary>
        [HttpGet("match/{matchId}/player-rankings")]
        public async Task<ActionResult> GetPlayerPerformanceComparison(string matchId)
        {
            var query = new GetPlayerPerformanceComparisonQuery { MatchId = matchId };
            var result = await _mediator.Send(query);
            return CreateResponse(result);
        }

        /// <summary>
        /// SLOŽEN UPIT 3: Season per-player averages across multiple matches
        /// Kombinuje filtriranje, grupisanje, agregaciju i sortiranje sa statističkim funkcijama
        /// </summary>
        /// <param name="startDate">Start date for season analysis</param>
        /// <param name="endDate">End date for season analysis</param>
        /// <param name="teamId">Optional: Filter by specific team</param>
        /// <param name="minMatches">Optional: Minimum matches played (default: 1)</param>
        /// <returns>List of players with season averages and statistics</returns>
        [HttpGet("season-averages")]
        public async Task<ActionResult> GetSeasonPlayerAverages(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string? teamId = null,
            [FromQuery] int minMatches = 1)
        {
            var query = new GetSeasonPlayerAveragesQuery 
            { 
                StartDate = startDate,
                EndDate = endDate,
                TeamId = teamId,
                MinMatches = minMatches
            };
            var result = await _mediator.Send(query);
            return CreateResponse(result);
        }

        /// <summary>
        /// INFLUX REPORT 1: Lista događaja za određenu utakmicu pri čemu je event_type '+2p', '+3p', '+ft' i event_category je 'personal' sortirano po vremenu
        /// </summary>
        /// <param name="matchId">Match identifier</param>
        /// <returns>List of scoring events sorted by time</returns>
        [HttpGet("reports/match/{matchId}/scoring-events")]
        public async Task<ActionResult> GetMatchScoringEventsInfluxReport(string matchId)
        {
            var query = new GetMatchScoringEventsInfluxReportQuery(matchId);
            var result = await _mediator.Send(query);
            
            if (result.Success)
            {
                return Ok(result.ScoringEvents);
            }
            
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// INFLUX REPORT 2: Prikaz broja događaja za svaki event_type (gde je event_category personal), grupisano po match-u za određenog igrača
        /// </summary>
        /// <param name="playerId">Player identifier</param>
        /// <param name="startDate">Start date for analysis</param>
        /// <param name="endDate">End date for analysis</param>
        /// <returns>Player event counts grouped by match</returns>
        [HttpGet("reports/player/{playerId}/event-counts")]
        public async Task<ActionResult> GetPlayerEventCountsInfluxReport(
            string playerId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var query = new GetPlayerEventCountsInfluxReportQuery(playerId, startDate, endDate);
            var result = await _mediator.Send(query);
            
            if (result.Success)
            {
                return Ok(result.PlayerEventCounts);
            }
            
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// INFLUX REPORT 3: Za zadati vremenski period (teamId = 1) vratiti za svakog igrača broj odigranih utakmica, avgPoints, avgAssists, avgFouls
        /// </summary>
        /// <param name="startDate">Start date for analysis</param>
        /// <param name="endDate">End date for analysis</param>
        /// <param name="teamId">Optional: Team identifier (default: "1")</param>
        /// <returns>Team player averages for the specified period</returns>
        [HttpGet("reports/team/player-averages")]
        public async Task<ActionResult> GetTeamPlayerAveragesInfluxReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string? teamId = "1")
        {
            var query = new GetTeamPlayerAveragesInfluxReportQuery(startDate, endDate, teamId);
            var result = await _mediator.Send(query);
            
            if (result.Success)
            {
                return Ok(result.TeamPlayerAverages);
            }
            
            return BadRequest(result.ErrorMessage);
        }
    }
}