using MediatR;
using Microsoft.AspNetCore.Mvc;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Features.TravelInformations.CreateTravelInfo;
using travel_service.src.Travels.Core.Application.Features.TravelInformations.GetAllTravelInfos;
using travel_service.src.Travels.Core.Application.Features.TravelInformations.GetTravelInfoById;
using travel_service.src.Travels.Core.Application.Features.TravelInformations.UpdateTravelInfo;

namespace travel_service.src.Travels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TravelInformationsController : BaseController
{
    private readonly IMediator _mediator;

    public TravelInformationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{userId}")]
    public async Task<ActionResult> CreateTravelInformation(int userId, [FromBody] CreateTravelInfoCommand command)
    {
        command.UserId = userId;
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpPut("{travelInfoId}/{userId}")]
    public async Task<ActionResult> UpdateTravelInformation(int travelInfoId, int userId, [FromBody] UpdateTravelInfoCommand command)
    {
        command.Id = travelInfoId;
        command.UserId = userId;
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpGet]
    public async Task<ActionResult> GetAllTravelInformations()
    {
        var query = new GetAllTravelInfosQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetTravelInformationById(int id)
    {
        var query = new GetTravelInfoByIdQuery(id);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

}
