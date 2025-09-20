using MediatR;
using Microsoft.AspNetCore.Mvc;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Features.Agencies.GetAllAgencies;
using travel_service.src.Travels.Core.Application.Features.Players.GetAllPlayers;

namespace travel_service.src.Travels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgenciesController : BaseController
{
    private readonly IMediator _mediator;

    public AgenciesController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet("{type}")]
    public async Task<ActionResult> GetAllAgencies(string type)
    {
        var query = new GetAllAgenciesQuery(type);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

}
