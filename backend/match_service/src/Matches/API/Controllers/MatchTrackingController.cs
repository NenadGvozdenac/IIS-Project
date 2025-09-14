using MediatR;
using Microsoft.AspNetCore.Mvc;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Features.MatchTracking.StartMatchOrPeriod;
using match_service.src.Matches.Core.Application.Features.MatchTracking.NextPeriod;
using match_service.src.Matches.Core.Application.Features.MatchTracking.PauseMatch;
using match_service.src.Matches.Core.Application.Features.MatchTracking.ResumeMatch;
using match_service.src.Matches.Core.Application.Features.MatchTracking.EndPeriod;
using match_service.src.Matches.Core.Application.Features.MatchTracking.EndMatch;
using match_service.src.Matches.Core.Application.Features.MatchTracking.Timeout;
using match_service.src.Matches.Core.Application.Features.MatchTracking.GetMatchEvents;

namespace match_service.src.Matches.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchTrackingController : BaseController
{
    private readonly IMediator _mediator;

    public MatchTrackingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{matchId}/start")]
    public async Task<ActionResult> StartMatchOrPeriod(int matchId)
    {
        var result = await _mediator.Send(new StartMatchOrPeriodCommand { MatchId = matchId });
        return CreateResponse(result);
    }

    [HttpPost("{matchId}/next-period")]
    public async Task<ActionResult> NextPeriod(int matchId)
    {
        var result = await _mediator.Send(new NextPeriodCommand { MatchId = matchId });
        return CreateResponse(result);
    }

    [HttpPost("{matchId}/pause")]
    public async Task<ActionResult> PauseMatch(int matchId)
    {
        var result = await _mediator.Send(new PauseMatchCommand { MatchId = matchId });
        return CreateResponse(result);
    }

    [HttpPost("{matchId}/resume")]
    public async Task<ActionResult> ResumeMatch(int matchId)
    {
        var result = await _mediator.Send(new ResumeMatchCommand { MatchId = matchId });
        return CreateResponse(result);
    }

    [HttpPost("{matchId}/end-period")]
    public async Task<ActionResult> EndPeriod(int matchId)
    {
        var result = await _mediator.Send(new EndPeriodCommand { MatchId = matchId });
        return CreateResponse(result);
    }

    [HttpPost("{matchId}/end")]
    public async Task<ActionResult> EndMatch(int matchId)
    {
        var result = await _mediator.Send(new EndMatchCommand { MatchId = matchId });
        return CreateResponse(result);
    }

    [HttpPost("{matchId}/timeout")]
    public async Task<ActionResult> Timeout(int matchId, [FromBody] TimeoutRequest request)
    {
        var command = new TimeoutCommand { MatchId = matchId, TeamId = request.TeamId };
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpGet("{matchId}/events")]
    public async Task<ActionResult> GetMatchEvents(int matchId)
    {
        var command = new GetMatchEventsCommand { MatchId = matchId };
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}

public class TimeoutRequest
{
    public int TeamId { get; set; }
}