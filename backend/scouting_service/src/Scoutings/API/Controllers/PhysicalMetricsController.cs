using MediatR;
using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Features.PhysicalMetrics.GetAllPhysicalMetrics;
using scouting_service.src.Scoutings.Core.Application.Features.PhysicalMetrics.CreatePhysicalMetric;

namespace scouting_service.src.Scoutings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PhysicalMetricsController : BaseController
{
    private readonly IMediator _mediator;

    public PhysicalMetricsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var query = new GetAllPhysicalMetricsQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreatePhysicalMetricCommand command)
    {
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}
