using MediatR;
using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Features.Metrics.GetAllMetrics;
using scouting_service.src.Scoutings.Core.Application.Features.Metrics.CreateMetric;
using scouting_service.src.Scoutings.Core.Application.Features.Metrics.UpdateMetric;

namespace scouting_service.src.Scoutings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetricsController : BaseController
{
    private readonly IMediator _mediator;

    public MetricsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var query = new GetAllMetricsQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateMetricCommand command)
    {
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateMetricCommand command)
    {
        command.IdMetrics = id;
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}
