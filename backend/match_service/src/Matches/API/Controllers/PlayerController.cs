using MediatR;
using Microsoft.AspNetCore.Mvc;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Features.Player.GetPlayerById;
using match_service.src.Matches.Core.Application.Features.Player.CreatePlayer;
using match_service.src.Matches.Core.Application.Features.Player.CreatePlayerWithTeamMember;

namespace match_service.src.Matches.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : BaseController
{
    private readonly IMediator _mediator;

    public PlayerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetPlayerById(int id)
    {
        var query = new GetPlayerByIdQuery(id);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreatePlayer([FromBody] CreatePlayerCommand command)
    {
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpPost("with-team-member")]
    public async Task<ActionResult> CreatePlayerWithTeamMember([FromBody] CreatePlayerWithTeamMemberCommand command)
    {
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}
