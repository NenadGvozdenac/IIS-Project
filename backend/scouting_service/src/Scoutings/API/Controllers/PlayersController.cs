using MediatR;
using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Features.Players.GetAllPlayers;
using scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerById;
using scouting_service.src.Scoutings.Core.Application.Features.Players.CreatePlayer;
using scouting_service.src.Scoutings.Core.Application.Features.Players.UpdatePlayer;
using scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerSeasonMetricAverages;
using scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerSessions;

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

    [HttpGet("{playerId}/season/{seasonId}/metrics/averages")]
    public async Task<ActionResult> GetPlayerSeasonMetricAverages(int playerId, int seasonId, [FromQuery] string? sessionType = null)
    {
        var query = new GetPlayerSeasonMetricAveragesQuery(playerId, seasonId, sessionType);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("{playerId}/sessions")]
    public async Task<ActionResult> GetPlayerSessions(
        int playerId,
        [FromQuery] int? seasonId = null,
        [FromQuery] string? status = null,
        [FromQuery] string? dateFrom = null,
        [FromQuery] string? dateTo = null)
    {
        var query = new GetPlayerSessionsQuery(playerId, seasonId, status, dateFrom, dateTo);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

}
