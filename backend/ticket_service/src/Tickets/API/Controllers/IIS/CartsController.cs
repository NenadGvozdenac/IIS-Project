using MediatR;
using Microsoft.AspNetCore.Mvc;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Features.Relational.Carts.GetAllCarts;
using ticket_service.src.Tickets.Core.Application.Features.Relational.Carts.GetCartById;
using ticket_service.src.Tickets.Core.Application.Features.Relational.Carts.GetCartsByUser;
using ticket_service.src.Tickets.Core.Application.Features.Relational.Carts.GetCurrentCart;
using ticket_service.src.Tickets.Core.Application.Features.Relational.Carts.PurchaseCart;

namespace ticket_service.src.Tickets.API.Controllers.IIS;

[ApiController]
[Route("api/[controller]")]
public class CartsController : BaseController
{
    private readonly IMediator _mediator;

    public CartsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllCarts()
    {
        var query = new GetAllCartsQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetCartById(int id)
    {
        var query = new GetCartByIdQuery(id);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult> GetCartsByUser(int userId)
    {
        var query = new GetCartsByUserQuery { IdUser = userId };
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("user/{userId}/current")]
    public async Task<ActionResult> GetCurrentCartByUser(int userId)
    {
        var query = new GetCurrentCartQuery { IdUser = userId };
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPost("{cartId}/purchase")]
    public async Task<ActionResult> PurchaseCart(int cartId, [FromBody] PurchaseCartRequest request)
    {
        var command = new PurchaseCartCommand
        {
            IdCart = cartId,
            IdCreditCard = request.IdCreditCard
        };

        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}

public class PurchaseCartRequest
{
    public int IdCreditCard { get; set; }
}
