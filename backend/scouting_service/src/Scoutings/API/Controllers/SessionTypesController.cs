using MediatR;
using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Features.SessionTypes.GetAllSessionTypes;
using scouting_service.src.Scoutings.Core.Application.Features.SessionTypes.CreateSessionType;

namespace scouting_service.src.Scoutings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionTypesController : BaseController
{
    private readonly IMediator _mediator;

    public SessionTypesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var query = new GetAllSessionTypesQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateSessionTypeCommand command)
    {
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}
