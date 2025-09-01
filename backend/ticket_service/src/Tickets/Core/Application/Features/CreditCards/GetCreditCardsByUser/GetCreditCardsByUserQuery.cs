using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.CreditCards.GetCreditCardsByUser;

public class GetCreditCardsByUserQuery : IRequest<Result<List<GetCreditCardsByUserResponse>>>
{
    public int IdUser { get; set; }
}
