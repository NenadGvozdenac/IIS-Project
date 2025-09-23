using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.CreditCards.GetCreditCardById;

public class GetCreditCardByIdQuery : IRequest<Result<GetCreditCardByIdResponse>>
{
    public int IdCreditCard { get; set; }

    public GetCreditCardByIdQuery(int idCreditCard)
    {
        IdCreditCard = idCreditCard;
    }
}
