using MediatR;
using Microsoft.AspNetCore.Mvc;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Reports.GetMatchTicketSalesReport;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Reports.GetCustomerSpendingReport;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Reports.GetSectorAnalysisReport;

namespace ticket_service.src.Tickets.API.Controllers.NAIS;

[ApiController]
[Route("api/neo4j/[controller]")]
public class ReportsController : BaseController
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("match-ticket-sales")]
    public async Task<IActionResult> GetMatchTicketSalesReport()
    {
        var query = new GetMatchTicketSalesReportQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("customer-spending")]
    public async Task<IActionResult> GetCustomerSpendingReport()
    {
        var query = new GetCustomerSpendingReportQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("sector-analysis")]
    public async Task<IActionResult> GetSectorAnalysisReport()
    {
        var query = new GetSectorAnalysisReportQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }
}