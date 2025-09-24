using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Features.Sessions.GetAllSessions;
using scouting_service.src.Scoutings.Core.Application.Features.Sessions.GetSessionById;
using scouting_service.src.Scoutings.Core.Application.Features.Sessions.CreateSession;
using scouting_service.src.Scoutings.Core.Application.Features.Sessions.UpdateSession;

namespace scouting_service.src.Scoutings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SessionsController : BaseController
{
    private readonly IMediator _mediator;

    public SessionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var query = new GetAllSessionsQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        var query = new GetSessionByIdQuery(id);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateSessionCommand command)
    {
        // Extract user ID from JWT claims
        var userIdClaim = User.FindFirst("userID")?.Value;
        if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
        {
            command.IdUser = userId;
        }
        
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateSessionCommand command)
    {
        command.IdSession = id;
        
        // Extract user ID from JWT claims for update as well
        var userIdClaim = User.FindFirst("userID")?.Value;
        if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
        {
            command.IdUser = userId;
        }
        
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}
