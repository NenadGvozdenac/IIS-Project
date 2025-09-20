using MediatR;
using Microsoft.AspNetCore.Mvc;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Features.Requests.GetAllRequests;
using travel_service.src.Travels.Core.Application.Features.Requests.CreateRequests;

namespace travel_service.src.Travels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RequestsController : BaseController
{
    private readonly IMediator _mediator;

    public RequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{type}/{idMatch}")]
    public async Task<ActionResult> GetAllRequests(string type, int idMatch)
    {
        var query = new GetAllRequestsQuery(type, idMatch);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPost("{type}/{userId}")]
    public async Task<ActionResult> CreateRequest(string type, int userId, [FromBody] CreateRequestsCommand command)
    {
        command.Type = type;
        command.UserId = userId;
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}
