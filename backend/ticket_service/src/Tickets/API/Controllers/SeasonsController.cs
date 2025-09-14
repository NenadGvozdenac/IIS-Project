using MediatR;
using Microsoft.AspNetCore.Mvc;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Features.Seasons.CreateSeason;
using ticket_service.src.Tickets.Core.Application.Features.Seasons.GetAllSeasons;
using ticket_service.src.Tickets.Core.Application.Features.Seasons.GetSeasonById;
using ticket_service.src.Tickets.Core.Application.Features.Seasons.UpdateSeason;
using ticket_service.src.Tickets.Core.Application.Features.Seasons.DeleteSeason;
using ticket_service.src.Tickets.Core.Application.Features.Seasons.EnableSeasonCards;

namespace ticket_service.src.Tickets.API.Controllers;

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

    [HttpPost]
    public async Task<ActionResult> CreateSeason([FromBody] CreateSeasonCommand command)
    {
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateSeason(int id, [FromBody] UpdateSeasonRequest request)
    {
        var command = new UpdateSeasonCommand
        {
            IdSeason = id,
            StartedAt = request.StartedAt,
            EndedAt = request.EndedAt,
            Name = request.Name
        };
        
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSeason(int id)
    {
        var command = new DeleteSeasonCommand(id);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpPost("{id}/enable-tickets")]
    public async Task<ActionResult> EnableSeasonTickets(int id, [FromBody] EnableSeasonTicketsRequest request)
    {
        var command = new EnableSeasonTicketsCommand 
        { 
            IdSeason = id,
            ZonePrices = request.ZonePrices.Select(zp => new ZonePricing
            {
                ZoneId = zp.ZoneId,
                Price = zp.Price
            }).ToList()
        };
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}

public class UpdateSeasonRequest
{
    public DateOnly StartedAt { get; set; }
    public DateOnly? EndedAt { get; set; }
    public string Name { get; set; } = null!;
}

public class EnableSeasonTicketsRequest
{
    public List<ZonePricingDto> ZonePrices { get; set; } = new List<ZonePricingDto>();
}

public class ZonePricingDto
{
    public int ZoneId { get; set; }
    public int Price { get; set; } // Price in cents
}
