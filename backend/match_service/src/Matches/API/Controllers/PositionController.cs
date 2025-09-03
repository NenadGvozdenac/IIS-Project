using MediatR;
using Microsoft.AspNetCore.Mvc;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Features.Position.GetAllPositions;
using match_service.src.Matches.Core.Application.Features.Position.GetPositionById;

namespace match_service.src.Matches.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PositionController : BaseController
{
    private readonly IMediator _mediator;

    public PositionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetPositionById(int id)
    {
        var query = new GetPositionByIdQuery(id);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet]
    public async Task<ActionResult> GetAllPositions()
    {
        var query = new GetAllPositionsQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }
}
