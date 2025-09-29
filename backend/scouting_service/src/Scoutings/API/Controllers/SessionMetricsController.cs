using MediatR;
using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Features.SessionMetrics.GetAllSessionMetrics;
using scouting_service.src.Scoutings.Core.Application.Features.SessionMetrics.CreateSessionMetric;
using scouting_service.src.Scoutings.Core.Application.Features.SessionMetrics.UpdateSessionMetric;

namespace scouting_service.src.Scoutings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionMetricsController : BaseController
{
    private readonly IMediator _mediator;

    public SessionMetricsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var query = new GetAllSessionMetricsQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateSessionMetricCommand command)
    {
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpPut("{sessionId}/{metricId}")]
    public async Task<ActionResult> Update(int sessionId, int metricId, [FromBody] UpdateSessionMetricCommand command)
    {
        command.IdSession = sessionId;
        command.IdMetrics = metricId;
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}
