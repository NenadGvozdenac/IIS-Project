using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.CreditCards.DeleteCreditCard;

public class DeleteCreditCardCommand : IRequest<Result<DeleteCreditCardResponse>>
{
    public int IdCreditCard { get; set; }

    public DeleteCreditCardCommand(int idCreditCard)
    {
        IdCreditCard = idCreditCard;
    }
}
