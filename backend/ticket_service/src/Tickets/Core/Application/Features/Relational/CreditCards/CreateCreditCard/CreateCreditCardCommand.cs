using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.CreditCards.CreateCreditCard;

public class CreateCreditCardCommand : IRequest<Result<CreateCreditCardResponse>>
{
    public string? Number { get; set; }
    public int? Cvv { get; set; }
    public string? Name { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public int IdUser { get; set; }
}
