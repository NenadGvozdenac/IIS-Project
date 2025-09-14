using MediatR;
using Microsoft.AspNetCore.Mvc;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Features.Matches.GetAllMatches;
using ticket_service.src.Tickets.Core.Application.Features.Matches.GetMatchById;
using ticket_service.src.Tickets.Core.Application.Features.Matches.GetMatchesInOurHall;
using ticket_service.src.Tickets.Core.Application.Features.Matches.EnableTicketsForUpcomingMatches;
using ticket_service.src.Tickets.Core.Application.Features.Matches.CreateTicketPriceParameter;

namespace ticket_service.src.Tickets.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchesController : BaseController
{
    private readonly IMediator _mediator;

    public MatchesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllMatches()
    {
        var query = new GetAllMatchesQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetMatchById(int id)
    {
        var query = new GetMatchByIdQuery(id);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("in-our-hall")]
    public async Task<ActionResult> GetMatchesInOurHall()
    {
        var query = new GetMatchesInOurHallQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPost("{id}/enable-tickets")]
    public async Task<ActionResult> EnableTicketsForUpcomingMatch(int id)
    {
        var command = new EnableTicketsForUpcomingMatchesQuery(id);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpPost("{id}/create-price-parameters")]
    public async Task<ActionResult> CreateTicketPriceParameter(int id, [FromBody] CreatePriceParameterRequest request)
    {
        var command = new CreateTicketPriceParameterQuery(
            id,
            request.ZoneId,
            request.PriceFactor,
            request.TimeFactor,
            request.MinimumSeatPrice,
            request.MaximumSeatPrice,
            request.UserId
        );
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}

public class CreatePriceParameterRequest
{
    public int ZoneId { get; set; }
    public int PriceFactor { get; set; }
    public int TimeFactor { get; set; }
    public int MinimumSeatPrice { get; set; }
    public int MaximumSeatPrice { get; set; }
    public int UserId { get; set; }
}
