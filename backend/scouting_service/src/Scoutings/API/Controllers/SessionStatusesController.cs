using MediatR;
using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Features.SessionStatuses.GetAllSessionStatuses;
using scouting_service.src.Scoutings.Core.Application.Features.SessionStatuses.CreateSessionStatus;

namespace scouting_service.src.Scoutings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionStatusesController : BaseController
{
    private readonly IMediator _mediator;

    public SessionStatusesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var query = new GetAllSessionStatusesQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateSessionStatusCommand command)
    {
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}
