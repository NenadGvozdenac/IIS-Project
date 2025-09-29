using MediatR;
using Microsoft.AspNetCore.Mvc;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Features.Matches.CreateMatch;
using travel_service.src.Travels.Core.Application.Features.Matches.DeleteMatch;
using travel_service.src.Travels.Core.Application.Features.Matches.GetAllMatches;
using travel_service.src.Travels.Core.Application.Features.Matches.GetMatchById;
using travel_service.src.Travels.Core.Application.Features.Matches.UpdateMatch;

namespace travel_service.src.Travels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchesController : BaseController
{
    private readonly IMediator _mediator;

    public MatchesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{userId}")]
    public async Task<ActionResult> CreateMatch(int userId, [FromBody] CreateMatchCommand command)
    {
        command.UserId = userId;
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpPut("{matchId}/{userId}")]
    public async Task<ActionResult> UpdateMatch(int matchId, int userId, [FromBody] UpdateMatchCommand command)
    {
        command.Id = matchId;
        command.UserId = userId;
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpDelete("{matchId}/{userId}")]
    public async Task<ActionResult> DeleteMatch(int matchId, int userId)
    {
        var command = new DeleteMatchCommand(matchId, userId);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
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

}
