using MediatR;
using Microsoft.AspNetCore.Mvc;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Features.TeamMember.GetAllTeamMembers;
using match_service.src.Matches.Core.Application.Features.TeamMember.GetTeamMemberById;
using match_service.src.Matches.Core.Application.Features.TeamMember.CreateTeamMember;
using match_service.src.Matches.Core.Application.Features.TeamMember.UpdateTeamMember;
using match_service.src.Matches.Core.Application.Features.TeamMember.DeleteTeamMember;

namespace match_service.src.Matches.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamMemberController : BaseController
{
    private readonly IMediator _mediator;

    public TeamMemberController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{idPlayer}/{idTeam}")]
    public async Task<ActionResult> GetTeamMemberById(int idPlayer, int idTeam)
    {
        var query = new GetTeamMemberByIdQuery(idPlayer, idTeam);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet]
    public async Task<ActionResult> GetAllTeamMembers()
    {
        var query = new GetAllTeamMembersQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateTeamMember([FromBody] CreateTeamMemberCommand command)
    {
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpPut("{idPlayer}/{idTeam}")]
    public async Task<ActionResult> UpdateTeamMember(int idPlayer, int idTeam, [FromBody] UpdateTeamMemberCommand command)
    {
        command.IdPlayer = idPlayer;
        command.IdTeam = idTeam;
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpDelete("{idPlayer}/{idTeam}")]
    public async Task<ActionResult> DeleteTeamMember(int idPlayer, int idTeam)
    {
        var command = new DeleteTeamMemberCommand(idPlayer, idTeam);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}
