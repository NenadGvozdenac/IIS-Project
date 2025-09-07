using MediatR;
using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Features.Positions.GetAllPositions;
using scouting_service.src.Scoutings.Core.Application.Features.Positions.CreatePosition;

namespace scouting_service.src.Scoutings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PositionsController : BaseController
{
    private readonly IMediator _mediator;

    public PositionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var query = new GetAllPositionsQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreatePositionCommand command)
    {
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}
