using MediatR;
using Microsoft.AspNetCore.Mvc;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Features.Zones.GetAllZones;
using ticket_service.src.Tickets.Core.Application.Features.Zones.GetZoneById;
using ticket_service.src.Tickets.Core.Application.Features.Zones.CreateZone;
using ticket_service.src.Tickets.Core.Application.Features.Zones.UpdateZone;
using ticket_service.src.Tickets.Core.Application.Features.Zones.DeleteZone;

namespace ticket_service.src.Tickets.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ZonesController : BaseController
{
    private readonly IMediator _mediator;

    public ZonesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllZones()
    {
        var query = new GetAllZonesQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetZoneById(int id)
    {
        var query = new GetZoneByIdQuery(id);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateZone([FromBody] CreateZoneCommand command)
    {
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateZone(int id, [FromBody] UpdateZoneRequest request)
    {
        var command = new UpdateZoneCommand
        {
            IdZone = id,
            Name = request.Name,
            Rank = request.Rank,
            MaximumCapacity = request.MaximumCapacity,
            Status = request.Status
        };
        
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteZone(int id)
    {
        var command = new DeleteZoneCommand(id);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}
