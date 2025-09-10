using MediatR;
using Microsoft.AspNetCore.Mvc;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.GetAllPurchaseOffers;
using ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.GetSeasonTickets;
using ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.GetIndividualTickets;
using ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.AddToCart;
using ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.RemoveFromCart;
using ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.GetSeasonTicketBySeat;
using ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.GetIndividualTicketBySeat;
using ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.GetUserPurchaseHistory;
using ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.CheckSeasonTicketConflict;
using ticket_service.src.Tickets.Core.Application.Features.PurchaseOffers.GetExistingSeasonTickets;

namespace ticket_service.src.Tickets.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseOffersController : BaseController
{
    private readonly IMediator _mediator;

    public PurchaseOffersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllPurchaseOffers()
    {
        var query = new GetAllPurchaseOffersQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("season-tickets")]
    public async Task<ActionResult> GetSeasonTickets()
    {
        var query = new GetSeasonTicketsQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("individual-tickets/{matchId}")]
    public async Task<ActionResult> GetIndividualTickets(int matchId)
    {
        var query = new GetIndividualTicketsQuery(matchId);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPost("{purchaseOfferId}/add-to-cart")]
    public async Task<ActionResult> AddToCart(int purchaseOfferId, [FromBody] AddToCartRequest request)
    {
        var command = new AddToCartCommand(purchaseOfferId, request.UserId);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpDelete("{purchaseOfferId}/remove-from-cart")]
    public async Task<ActionResult> RemoveFromCart(int purchaseOfferId, [FromBody] RemoveFromCartRequest request)
    {
        var command = new RemoveFromCartCommand(request.UserId, purchaseOfferId);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpGet("season-ticket/zone/{zoneId}/row/{row}/seat/{number}/direction/{direction}/season/{seasonId}")]
    public async Task<ActionResult> GetSeasonTicketBySeat(int zoneId, int row, int number, string direction, int seasonId)
    {
        var query = new GetSeasonTicketBySeatQuery(zoneId, row, number, direction, seasonId);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("individual-ticket/zone/{zoneId}/row/{row}/seat/{number}/direction/{direction}/match/{matchId}")]
    public async Task<ActionResult> GetIndividualTicketBySeat(int zoneId, int row, int number, string direction, int matchId)
    {
        var query = new GetIndividualTicketBySeatQuery(zoneId, row, number, direction, matchId);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("purchase-history/{userId}")]
    public async Task<ActionResult> GetUserPurchaseHistory(int userId)
    {
        var query = new GetUserPurchaseHistoryQuery(userId);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("season-ticket-conflict/seat/{seatId}/match-date/{matchDate}")]
    public async Task<ActionResult> CheckSeasonTicketConflict(int seatId, string matchDate)
    {
        var query = new CheckSeasonTicketConflictQuery(seatId, matchDate);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("existing-season-tickets/zone/{zoneId}/season/{seasonId}")]
    public async Task<ActionResult> GetExistingSeasonTickets(int zoneId, int seasonId)
    {
        var query = new GetExistingSeasonTicketsQuery(zoneId, seasonId);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }
}

public class AddToCartRequest
{
    public int UserId { get; set; }
}

public class RemoveFromCartRequest
{
    public int UserId { get; set; }
}
