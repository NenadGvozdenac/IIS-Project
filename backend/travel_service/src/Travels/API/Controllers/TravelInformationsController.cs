using MediatR;
using Microsoft.AspNetCore.Mvc;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TravelInformationsController : BaseController
{
    private readonly IMediator _mediator;

    public TravelInformationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /*[HttpPost("{userId}")]
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
    }*/

}
