using MediatR;
using Microsoft.AspNetCore.Mvc;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Features.Teams.GetTeamById;
using match_service.src.Matches.Core.Application.Features.Teams.GetAllTeams;
using match_service.src.Matches.Core.Application.Features.Teams.CreateTeam;
using match_service.src.Matches.Core.Application.Features.Teams.UpdateTeam;
using match_service.src.Matches.Core.Application.Features.Teams.DeleteTeam;

namespace match_service.src.Matches.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamController : BaseController
{
    private readonly IMediator _mediator;

    public TeamController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetTeamById(int id)
    {
        var query = new GetTeamByIdQuery(id);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet]
    public async Task<ActionResult> GetAllTeams()
    {
        var query = new GetAllTeamsQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateTeam([FromBody] CreateTeamCommand command)
    {
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateTeam(int id, [FromBody] UpdateTeamCommand command)
    {
        command.IdTeam = id;
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTeam(int id)
    {
        var command = new DeleteTeamCommand(id);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}