using MediatR;
using Microsoft.AspNetCore.Mvc;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Features.Matches.GetAllMatches;
using ticket_service.src.Tickets.Core.Application.Features.Matches.GetMatchById;
using ticket_service.src.Tickets.Core.Application.Features.Matches.GetMatchesInOurHall;

namespace ticket_service.src.Tickets.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchesController : BaseController
{
    private readonly IMediator _mediator;

    public MatchesController(IMediator mediator)
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

    [HttpGet("in-our-hall")]
    public async Task<ActionResult> GetMatchesInOurHall()
    {
        var query = new GetMatchesInOurHallQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }
}
