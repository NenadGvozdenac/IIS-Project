using MediatR;
using Microsoft.AspNetCore.Mvc;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Features.Requests.GetAllRequests;
using travel_service.src.Travels.Core.Application.Features.Requests.CreateRequests;
using travel_service.src.Travels.Core.Application.Features.Requests.GetRequestsByMatch;
using travel_service.src.Travels.Core.Application.Features.Requests.CheckExistingRequest;

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

    [HttpGet("by-match/{matchId}")]
    public async Task<ActionResult> GetRequestsByMatch(int matchId)
    {
        var query = new GetRequestsByMatchQuery(matchId);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("check-existing/{matchId}/{type}")]
    public async Task<ActionResult> CheckExistingRequest(int matchId, string type)
    {
        var query = new CheckExistingRequestQuery(matchId, type);
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
