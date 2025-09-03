using MediatR;
using Microsoft.AspNetCore.Mvc;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Features.Visas.CreateVisa;
using travel_service.src.Travels.Core.Application.Features.Visas.GetVisasByTravelInfoId;

namespace travel_service.src.Travels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VisasController : BaseController
{
    private readonly IMediator _mediator;

    public VisasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{userId}")]
    public async Task<ActionResult> CreateVisa(int userId, [FromBody] CreateVisaCommand command)
    {
        command.UserId = userId;
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpGet("by-travel-info/{travelInfoId}")]
    public async Task<ActionResult> GetVisasByTravelInfoId(int travelInfoId)
    {
        var query = new GetVisasByTravelInfoIdQuery(travelInfoId); // koristi novi query
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

}
