using MediatR;
using Microsoft.AspNetCore.Mvc;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Matches.GetAllMatches;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Matches.GetMatchByName;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Matches.GetMatchById;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Matches.CreateMatch;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Matches.UpdateMatch;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Matches.DeleteMatch;
using ticket_service.src.Tickets.Core.Application.DTOs.Graph;

namespace ticket_service.src.Tickets.API.Controllers.NAIS;

[ApiController]
[Route("api/neo4j/[controller]")]
public class MatchesController : BaseController
{
    private readonly IMediator _mediator;

    public MatchesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMatches()
    {
        var query = new GetAllGraphMatchesQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetMatchByName(string name)
    {
        var query = new GetGraphMatchByNameQuery(name);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetMatchById(int id)
    {
        var query = new GetGraphMatchByIdQuery(id);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMatch([FromBody] CreateMatchDto matchDto)
    {
        var command = new CreateGraphMatchCommand(
            matchDto.Name, 
            matchDto.ScheduledAt, 
            matchDto.Type, 
            matchDto.State, 
            matchDto.City, 
            matchDto.Hall, 
            matchDto.IsInOurHall);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpPut("id/{id}")]
    public async Task<IActionResult> UpdateMatch(int id, [FromBody] UpdateMatchDto matchDto)
    {
        var command = new UpdateGraphMatchCommand(
            id, 
            matchDto.Name, 
            matchDto.ScheduledAt, 
            matchDto.Type, 
            matchDto.State, 
            matchDto.City, 
            matchDto.Hall, 
            matchDto.IsInOurHall);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpDelete("id/{id}")]
    public async Task<IActionResult> DeleteMatch(int id)
    {
        var command = new DeleteGraphMatchCommand(id);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}