using MediatR;
using Microsoft.AspNetCore.Mvc;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Features.TeamMembers.GetAllTeamMembers;

namespace travel_service.src.Travels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamMembersController : BaseController
{
    private readonly IMediator _mediator;

    public TeamMembersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet]
    public async Task<ActionResult> GetAllTeamMembers()
    {
        var query = new GetAllTeamMembersQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

}
