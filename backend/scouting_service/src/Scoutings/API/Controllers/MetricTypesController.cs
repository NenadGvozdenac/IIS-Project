using MediatR;
using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Features.MetricTypes.GetAllMetricTypes;
using scouting_service.src.Scoutings.Core.Application.Features.MetricTypes.CreateMetricType;

namespace scouting_service.src.Scoutings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetricTypesController : BaseController
{
    private readonly IMediator _mediator;

    public MetricTypesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var query = new GetAllMetricTypesQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateMetricTypeCommand command)
    {
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}
