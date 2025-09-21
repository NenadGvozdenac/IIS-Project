using match_service.src.Matches.Core.Application.Features.ChronologicalEvent.CreateEventInflux;
using match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetMatchEventsInflux;
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
    }
}