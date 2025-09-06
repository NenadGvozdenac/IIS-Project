using MediatR;
using Microsoft.AspNetCore.Mvc;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Features.Competitions.GetAllCompetitions;
using travel_service.src.Travels.Core.Application.Features.Competitions.GetCompetitionById;

namespace ticket_service.src.Tickets.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompetitionsController : BaseController
{
    private readonly IMediator _mediator;

    public CompetitionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllCompetitions()
    {
        var query = new GetAllCompetitionsQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetCompetitionById(int id)
    {
        var query = new GetCompetitionByIdQuery(id);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }
}