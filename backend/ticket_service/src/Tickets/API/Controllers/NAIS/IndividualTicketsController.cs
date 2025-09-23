using MediatR;
using Microsoft.AspNetCore.Mvc;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Features.Graph.IndividualTickets.GetAllIndividualTickets;
using ticket_service.src.Tickets.Core.Application.Features.Graph.IndividualTickets.GetTicketById;
using ticket_service.src.Tickets.Core.Application.Features.Graph.IndividualTickets.CreateTicket;
using ticket_service.src.Tickets.Core.Application.Features.Graph.IndividualTickets.UpdateTicket;
using ticket_service.src.Tickets.Core.Application.Features.Graph.IndividualTickets.DeleteTicket;
using ticket_service.src.Tickets.Core.Application.DTOs.Graph;

namespace ticket_service.src.Tickets.API.Controllers.NAIS;

[ApiController]
[Route("api/neo4j/individual-tickets")]
public class IndividualTicketsController : BaseController
{
    private readonly IMediator _mediator;

    public IndividualTicketsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllIndividualTickets()
    {
        var query = new GetAllGraphIndividualTicketsQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetIndividualTicketById(int id)
    {
        var query = new GetGraphIndividualTicketByIdQuery(id);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateIndividualTicket([FromBody] CreateIndividualTicketDto ticketDto)
    {
        var command = new CreateGraphIndividualTicketCommand(
            ticketDto.Name,
            ticketDto.Description,
            ticketDto.Type,
            ticketDto.ReleasedAt,
            ticketDto.Price);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpPut("id/{id}")]
    public async Task<IActionResult> UpdateIndividualTicket(int id, [FromBody] UpdateIndividualTicketDto ticketDto)
    {
        var command = new UpdateGraphIndividualTicketCommand(
            id,
            ticketDto.Name,
            ticketDto.Description,
            ticketDto.Type,
            ticketDto.ReleasedAt,
            ticketDto.Price);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpDelete("id/{id}")]
    public async Task<IActionResult> DeleteIndividualTicket(int id)
    {
        var command = new DeleteGraphIndividualTicketCommand(id);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}