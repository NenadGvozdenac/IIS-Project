using MediatR;
using Microsoft.AspNetCore.Mvc;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Features.CreditCards.GetAllCreditCards;
using ticket_service.src.Tickets.Core.Application.Features.CreditCards.GetCreditCardById;
using ticket_service.src.Tickets.Core.Application.Features.CreditCards.GetCreditCardsByUser;
using ticket_service.src.Tickets.Core.Application.Features.CreditCards.CreateCreditCard;
using ticket_service.src.Tickets.Core.Application.Features.CreditCards.UpdateCreditCard;
using ticket_service.src.Tickets.Core.Application.Features.CreditCards.DeleteCreditCard;

namespace ticket_service.src.Tickets.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CreditCardsController : BaseController
{
    private readonly IMediator _mediator;

    public CreditCardsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllCreditCards()
    {
        var query = new GetAllCreditCardsQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetCreditCardById(int id)
    {
        var query = new GetCreditCardByIdQuery(id);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult> GetCreditCardsByUser(int userId)
    {
        var query = new GetCreditCardsByUserQuery { IdUser = userId };
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCreditCard([FromBody] CreateCreditCardCommand command)
    {
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCreditCard(int id, [FromBody] UpdateCreditCardRequest request)
    {
        var command = new UpdateCreditCardCommand
        {
            IdCreditCard = id,
            Number = request.Number,
            Cvv = request.Cvv,
            Name = request.Name,
            ExpirationDate = request.ExpirationDate
        };
        
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCreditCard(int id)
    {
        var command = new DeleteCreditCardCommand(id);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}
