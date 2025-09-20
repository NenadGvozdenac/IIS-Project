using match_service.src.Matches.Core.Application.Features.ChronologicalEvent.CreateEvent;
using match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetEventTypes;
using match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetTeamStatistics;
using match_service.src.Matches.Core.Application.Features.ChronologicalEvent.UndoLastEvent;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChronologicalEventController : BaseController
    {
        private readonly IMediator _mediator;

        public ChronologicalEventController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("types/{category}")]
        public async Task<ActionResult> GetEventTypes(string category)
        {
            var command = new GetEventTypesCommand { Category = category };
            var result = await _mediator.Send(command);
            return CreateResponse(result);
        }

        [HttpPost]
        public async Task<ActionResult> CreateEvent([FromBody] CreateEventCommand command)
        {
            var result = await _mediator.Send(command);
            return CreateResponse(result);
        }

        [HttpDelete("undo/match/{matchId}/team/{teamId}")]
        public async Task<ActionResult> UndoLastEvent(int matchId, int teamId)
        {
            var command = new UndoLastEventCommand { MatchId = matchId, TeamId = teamId };
            var result = await _mediator.Send(command);
            return CreateResponse(result);
        }

        [HttpGet("team-statistics/{teamId}")]
        public async Task<ActionResult> GetTeamStatistics(int teamId)
        {
            var query = new GetTeamStatisticsQuery { TeamId = teamId };
            var result = await _mediator.Send(query);
            return CreateResponse(result);
        }
    }
}