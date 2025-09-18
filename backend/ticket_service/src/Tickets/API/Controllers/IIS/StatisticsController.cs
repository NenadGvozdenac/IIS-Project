using MediatR;
using Microsoft.AspNetCore.Mvc;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Features.Relational.Statistics.GetMatchStatistics;

namespace ticket_service.src.Tickets.API.Controllers.IIS;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController : BaseController
{
    private readonly IMediator _mediator;

    public StatisticsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{matchId}")]
    public async Task<ActionResult> GetMatchStatistics(int matchId)
    {
        var query = new GetMatchStatisticsQuery(matchId);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }
}