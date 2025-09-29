using MediatR;
using Microsoft.AspNetCore.Mvc;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Features.Relational.Statistics.GetMatchStatistics;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

namespace ticket_service.src.Tickets.API.Controllers.IIS;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IStatisticsRepository _statisticsRepository;

    public StatisticsController(IMediator mediator, IStatisticsRepository statisticsRepository)
    {
        _mediator = mediator;
        _statisticsRepository = statisticsRepository;
    }

    [HttpGet("{matchId}")]
    public async Task<ActionResult> GetMatchStatistics(int matchId)
    {
        var query = new GetMatchStatisticsQuery(matchId);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("match-summary-report")]
    public async Task<ActionResult> GetMatchSummaryReport()
    {
        try
        {
            var result = await _statisticsRepository.GetMatchSummaryReportAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error generating match summary report: {ex.Message}");
        }
    }
}