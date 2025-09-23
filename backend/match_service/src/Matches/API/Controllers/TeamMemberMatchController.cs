using MediatR;
using Microsoft.AspNetCore.Mvc;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Features.TeamMemberMatch.GetTeamMembersByMatch;
using match_service.src.Matches.Core.Application.Features.TeamMemberMatch.UpdateStartingLineup;
using match_service.src.Matches.Core.Application.Features.TeamMemberMatch.PlayerSubstitution;

namespace match_service.src.Matches.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamMemberMatchController : BaseController
{
    private readonly IMediator _mediator;

    public TeamMemberMatchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("match/{matchId}")]
    public async Task<ActionResult> GetTeamMembersByMatch(int matchId, [FromQuery] int? teamId = null)
    {
        var query = new GetTeamMembersByMatchQuery(matchId, teamId);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPut("match/{matchId}/starting-lineup")]
    public async Task<ActionResult> UpdateStartingLineup(int matchId, [FromBody] List<StartingLineupPlayerRequest> players)
    {
        var command = new UpdateStartingLineupCommand(matchId, players);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpPost("match/{matchId}/substitution")]
    public async Task<ActionResult> PlayerSubstitution(int matchId, [FromBody] PlayerSubstitutionRequest request)
    {
        var command = new PlayerSubstitutionCommand
        {
            MatchId = matchId,
            TeamId = request.TeamId,
            PlayerInId = request.PlayerInId,
            PlayerOutId = request.PlayerOutId
        };
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}

public class PlayerSubstitutionRequest
{
    public int TeamId { get; set; }
    public int PlayerInId { get; set; }
    public int PlayerOutId { get; set; }
    public int? AnalystId { get; set; }
}
