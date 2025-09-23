using MediatR;
using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Features.Players.GetAllPlayers;
using scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerById;
using scouting_service.src.Scoutings.Core.Application.Features.Players.CreatePlayer;
using scouting_service.src.Scoutings.Core.Application.Features.Players.UpdatePlayer;

namespace scouting_service.src.Scoutings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : BaseController
{
    private readonly IMediator _mediator;

    public PlayersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var query = new GetAllPlayersQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        var query = new GetPlayerByIdQuery(id);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreatePlayerCommand command)
    {
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdatePlayerCommand command)
    {
        command.IdPlayer = id;
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

}
