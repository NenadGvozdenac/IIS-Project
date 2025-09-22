using MediatR;
using Microsoft.AspNetCore.Mvc;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Features.Trip.CreateTrip;
using travel_service.src.Travels.Core.Application.Features.Trip.GetTripByMatch;

namespace travel_service.src.Travels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripController : BaseController
{
    private readonly IMediator _mediator;

    public TripController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult> CreateTrip([FromBody] CreateTripCommand command)
    {
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpGet("match/{matchId}")]
    public async Task<ActionResult> GetTripByMatch(int matchId)
    {
        var query = new GetTripByMatchQuery(matchId);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }
}
