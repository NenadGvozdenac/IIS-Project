using MediatR;
using Microsoft.AspNetCore.Mvc;
using ticket_service.src.Tickets.Core.Application.Features.Graph.ComplexQueries.GetMatchRevenue;
using ticket_service.src.Tickets.Core.Application.Features.Graph.ComplexQueries.GetCustomerSpending;
using ticket_service.src.Tickets.Core.Application.Features.Graph.ComplexQueries.GetSectorSales;
using ticket_service.src.Tickets.Core.Application.Features.Graph.ComplexQueries.GetCustomerMatches;
using ticket_service.src.Tickets.Core.Application.Features.Graph.ComplexQueries.GetMatchAveragePrice;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.API.Controllers.NAIS;

[ApiController]
[Route("api/neo4j/[controller]")]
public class ComplexQueriesController : BaseController
{
    private readonly IMediator _mediator;

    public ComplexQueriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("match-revenue")]
    public async Task<IActionResult> GetMatchRevenue([FromQuery] int minTicketsSold = 2)
    {
        var query = new GetMatchRevenueQuery(minTicketsSold);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }
    
    [HttpGet("customer-spending")]
    public async Task<IActionResult> GetCustomerSpending([FromQuery] decimal minSpending = 4000)
    {
        var query = new GetCustomerSpendingQuery(minSpending);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("sector-sales/{matchId}")]
    public async Task<IActionResult> GetSectorSales(int matchId, [FromQuery] int minTicketsSold = 1)
    {
        var query = new GetSectorSalesQuery(matchId, minTicketsSold);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("customer-matches")]
    public async Task<IActionResult> GetCustomerMatches()
    {
        var query = new GetCustomerMatchesQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("match-average-price")]
    public async Task<IActionResult> GetMatchAveragePrice([FromQuery] int minTicketsSold = 1)
    {
        var query = new GetMatchAveragePriceQuery(minTicketsSold);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }
}