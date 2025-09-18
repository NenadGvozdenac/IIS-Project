using MediatR;
using Microsoft.AspNetCore.Mvc;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.GetAllSeats;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.GetSeatByName;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.GetSeatById;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.CreateSeat;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.UpdateSeat;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.DeleteSeat;
using ticket_service.src.Tickets.Core.Application.DTOs.Graph;

namespace ticket_service.src.Tickets.API.Controllers.NAIS;

[ApiController]
[Route("api/neo4j/[controller]")]
public class SeatsController : BaseController
{
    private readonly IMediator _mediator;

    public SeatsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSeats()
    {
        var query = new GetAllGraphSeatsQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetSeatByName(string name)
    {
        var query = new GetGraphSeatByNameQuery(name);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetSeatById(int id)
    {
        var query = new GetGraphSeatByIdQuery(id);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSeat([FromBody] CreateSeatDto seatDto)
    {
        var command = new CreateGraphSeatCommand(seatDto.Name, seatDto.Row, seatDto.Number, seatDto.Direction);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpPut("id/{id}")]
    public async Task<IActionResult> UpdateSeat(int id, [FromBody] UpdateSeatDto seatDto)
    {
        var command = new UpdateGraphSeatCommand(id, seatDto.Name, seatDto.Row, seatDto.Number, seatDto.Direction);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpDelete("id/{id}")]
    public async Task<IActionResult> DeleteSeat(int id)
    {
        var command = new DeleteGraphSeatCommand(id);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}