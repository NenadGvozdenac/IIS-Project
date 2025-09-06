using MediatR;
using Microsoft.AspNetCore.Mvc;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Features.Teams.GetAllTeams;
using travel_service.src.Travels.Core.Application.Features.Teams.GetTeamById;

namespace travel_service.src.Travels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : BaseController
{
    private readonly IMediator _mediator;

    public TeamsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet]
    public async Task<ActionResult> GetAllTeams()
    {
        var query = new GetAllTeamsQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetTeamById(int id)
    {
        var query = new GetTeamByIdQuery(id);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

}
