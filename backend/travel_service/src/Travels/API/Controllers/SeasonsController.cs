using MediatR;
using Microsoft.AspNetCore.Mvc;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Features.Seasons.GetAllSeasons;
using travel_service.src.Travels.Core.Application.Features.Seasons.GetSeasonById;

namespace travel_service.src.Travels.API.Controllers;

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
    public async Task<ActionResult> GetAllSeasons()
    {
        var query = new GetAllSeasonsQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetSeasonById(int id)
    {
        var query = new GetSeasonByIdQuery(id);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }
}
