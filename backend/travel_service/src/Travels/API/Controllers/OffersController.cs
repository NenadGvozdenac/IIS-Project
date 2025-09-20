using MediatR;
using Microsoft.AspNetCore.Mvc;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Features.Offers.CreateOffer;
using travel_service.src.Travels.Core.Application.Features.Offers.GetAllOffers;
using travel_service.src.Travels.Core.Application.Features.Offers.GetOfferByIds;

namespace travel_service.src.Travels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OffersController : BaseController
{
    private readonly IMediator _mediator;

    public OffersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{type}/{idMatch}")]
    public async Task<ActionResult> GetAllOffers(string type, int idMatch)
    {
        var query = new GetAllOffersQuery(type, idMatch);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPost("{type}/{userId}")]
    public async Task<ActionResult> CreateOffer(string type, int userId, [FromBody] CreateOfferCommand command)
    {
        command.Type = type;
        command.UserId = userId;
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpGet("chosen/{type}/{idMatch}")]
    public async Task<ActionResult> GetChosenOffer(string type, int idMatch)
    {
        var query = new GetOfferByIdsQuery(type, idMatch);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }
}
