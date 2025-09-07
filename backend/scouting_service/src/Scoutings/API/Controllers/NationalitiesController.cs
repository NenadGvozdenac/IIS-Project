using MediatR;
using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Features.Nationalities.GetAllNationalities;
using scouting_service.src.Scoutings.Core.Application.Features.Nationalities.CreateNationality;

namespace scouting_service.src.Scoutings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NationalitiesController : BaseController
{
    private readonly IMediator _mediator;

    public NationalitiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var query = new GetAllNationalitiesQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateNationalityCommand command)
    {
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}
