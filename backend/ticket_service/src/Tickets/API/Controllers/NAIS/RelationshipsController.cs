using MediatR;
using Microsoft.AspNetCore.Mvc;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Relationships.Bought.CreateBought;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Relationships.Bought.DeleteBought;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Relationships.IsForSeat.CreateIsForSeat;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Relationships.IsForSeat.DeleteIsForSeat;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Relationships.IsForMatch.CreateIsForMatch;
using ticket_service.src.Tickets.Core.Application.Features.Graph.Relationships.IsForMatch.DeleteIsForMatch;

namespace ticket_service.src.Tickets.API.Controllers.NAIS;

[ApiController]
[Route("api/neo4j/[controller]")]
public class RelationshipsController : BaseController
{
    private readonly IMediator _mediator;

    public RelationshipsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // BOUGHT Relationship endpoints
    [HttpPost("bought")]
    public async Task<IActionResult> CreateBoughtRelationship([FromBody] CreateBoughtRelationshipRequest request)
    {
        var command = new CreateBoughtRelationshipCommand(
            request.CustomerId, 
            request.IndividualTicketId, 
            request.PurchasedAt, 
            request.Price);
        
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpDelete("bought/{customerId}/{individualTicketId}")]
    public async Task<IActionResult> DeleteBoughtRelationship(int customerId, int individualTicketId)
    {
        var command = new DeleteBoughtRelationshipCommand(customerId, individualTicketId);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    // IS_FOR_SEAT Relationship endpoints
    [HttpPost("is-for-seat")]
    public async Task<IActionResult> CreateIsForSeatRelationship([FromBody] CreateIsForSeatRelationshipRequest request)
    {
        var command = new CreateIsForSeatRelationshipCommand(
            request.IndividualTicketId, 
            request.SeatId);
        
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpDelete("is-for-seat/{individualTicketId}/{seatId}")]
    public async Task<IActionResult> DeleteIsForSeatRelationship(int individualTicketId, int seatId)
    {
        var command = new DeleteIsForSeatRelationshipCommand(individualTicketId, seatId);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    // IS_FOR_MATCH Relationship endpoints
    [HttpPost("is-for-match")]
    public async Task<IActionResult> CreateIsForMatchRelationship([FromBody] CreateIsForMatchRelationshipRequest request)
    {
        var command = new CreateIsForMatchRelationshipCommand(
            request.IndividualTicketId, 
            request.MatchId);
        
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpDelete("is-for-match/{individualTicketId}/{matchId}")]
    public async Task<IActionResult> DeleteIsForMatchRelationship(int individualTicketId, int matchId)
    {
        var command = new DeleteIsForMatchRelationshipCommand(individualTicketId, matchId);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}

// Request DTOs
public class CreateBoughtRelationshipRequest
{
    public int CustomerId { get; set; }
    public int IndividualTicketId { get; set; }
    public DateTime PurchasedAt { get; set; }
    public decimal Price { get; set; }
}

public class CreateIsForSeatRelationshipRequest
{
    public int IndividualTicketId { get; set; }
    public int SeatId { get; set; }
}

public class CreateIsForMatchRelationshipRequest
{
    public int IndividualTicketId { get; set; }
    public int MatchId { get; set; }
}