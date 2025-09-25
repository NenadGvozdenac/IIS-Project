using MediatR;
using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Features.Seasons.GetAllSeasons;
using scouting_service.src.Scoutings.Core.Application.Features.Seasons.CreateSeason;

namespace scouting_service.src.Scoutings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeasonsController : BaseController
{
    private readonly IMediator _mediator;

    public SeasonsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var query = new GetAllSeasonsQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateSeasonCommand command)
    {
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}
