using MediatR;
using Microsoft.AspNetCore.Mvc;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Features.Relational.Seats.CreateSeat;
using ticket_service.src.Tickets.Core.Application.Features.Relational.Seats.DeleteSeat;
using ticket_service.src.Tickets.Core.Application.Features.Relational.Seats.GetAllSeats;
using ticket_service.src.Tickets.Core.Application.Features.Relational.Seats.GetSeatById;
using ticket_service.src.Tickets.Core.Application.Features.Relational.Seats.GetSeatsByZone;
using ticket_service.src.Tickets.Core.Application.Features.Relational.Seats.GetSeatsWithOffersForZoneAndDirection;
using ticket_service.src.Tickets.Core.Application.Features.Relational.Seats.UpdateSeat;

namespace ticket_service.src.Tickets.API.Controllers.IIS;

[ApiController]
[Route("api/[controller]")]
public class SeatsController : BaseController
{
    private readonly IMediator _mediator;

    public SeatsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllSeats()
    {
        var query = new GetAllSeatsQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetSeatById(int id)
    {
        var query = new GetSeatByIdQuery(id);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("zone/{zoneId}")]
    public async Task<ActionResult> GetSeatsByZone(int zoneId)
    {
        var query = new GetSeatsByZoneQuery(zoneId);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("zone/{zoneId}/direction/{direction}/match/{matchId}/with-offers")]
    public async Task<ActionResult> GetSeatsWithOffersForZoneAndDirection(int zoneId, string direction, int matchId)
    {
        var query = new GetSeatsWithOffersForZoneAndDirectionQuery(zoneId, direction, matchId);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateSeat([FromBody] CreateSeatCommand command)
    {
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateSeat(int id, [FromBody] UpdateSeatRequest request)
    {
        var command = new UpdateSeatCommand
        {
            IdSeat = id,
            Row = request.Row,
            Number = request.Number,
            Type = request.Type,
            Direction = request.Direction,
            Status = request.Status,
            IdZone = request.IdZone
        };

        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSeat(int id)
    {
        var command = new DeleteSeatCommand(id);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}
