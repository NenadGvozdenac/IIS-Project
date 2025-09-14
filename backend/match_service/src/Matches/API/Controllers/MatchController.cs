using MediatR;
using Microsoft.AspNetCore.Mvc;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Features.Match.GetAllMatches;
using match_service.src.Matches.Core.Application.Features.Match.GetMatchById;
using match_service.src.Matches.Core.Application.Features.MatchTracking.GetMatchTrackingByMatchId;
using match_service.src.Matches.Core.Application.Features.MatchTracking.PrepareMatchTracking;
using match_service.src.Matches.Core.Application.Features.TeamMemberMatch.GetTeamMembersByMatch;

namespace match_service.src.Matches.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchController : BaseController
{
    private readonly IMediator _mediator;

    public MatchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllMatches()
    {
        var query = new GetAllMatchesQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetMatchById(int id)
    {
        var query = new GetMatchByIdQuery(id);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("{matchId}/tracking")]
    public async Task<ActionResult> GetMatchTrackingByMatchId(int matchId)
    {
        var query = new GetMatchTrackingByMatchIdQuery(matchId);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("{matchId}/team-members")]
    public async Task<ActionResult> GetTeamMembersByMatch(int matchId, [FromQuery] int? teamId = null)
    {
        var query = new GetTeamMembersByMatchQuery(matchId, teamId);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPost("{matchId}/prepare")]
    public async Task<ActionResult> PrepareMatchTracking(int matchId, [FromBody] PrepareMatchTrackingRequest request)
    {
        var command = new PrepareMatchTrackingCommand
        {
            MatchId = matchId,
            OurTeamPlayerIds = request.OurTeamPlayerIds,
            OpponentTeamPlayerIds = request.OpponentTeamPlayerIds,
            AnalystId = request.AnalystId
        };
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}

public class PrepareMatchTrackingRequest
{
    public List<int> OurTeamPlayerIds { get; set; } = new List<int>();
    public List<int> OpponentTeamPlayerIds { get; set; } = new List<int>();
    public int? AnalystId { get; set; }
}
