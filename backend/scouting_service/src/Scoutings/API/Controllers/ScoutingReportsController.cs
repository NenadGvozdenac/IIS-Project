using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Features.Reports.GenerateScoutingReport;

namespace scouting_service.src.Scoutings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class ScoutingReportsController : BaseController
{
    private readonly IMediator _mediator;

    public ScoutingReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("generate-report")]
    public async Task<ActionResult> GenerateReport([FromBody] GenerateScoutingReportCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            var fileResult = result.Value as byte[];
            return File(fileResult, "application/pdf", $"scouting-report-{DateTime.Now:yyyy-MM-dd}.pdf");
        }
        
        return CreateResponse(result);
    }
}