using MediatR;
using Microsoft.AspNetCore.Mvc;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Features.ManagementMembers.GetAllMembers;

namespace travel_service.src.Travels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ManagementMembersController : BaseController
{
    private readonly IMediator _mediator;

    public ManagementMembersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet]
    public async Task<ActionResult> GetAllMembers()
    {
        var query = new GetAllMembersQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

}
